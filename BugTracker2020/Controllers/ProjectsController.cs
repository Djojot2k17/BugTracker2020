using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracker2020.Data;
using BugTracker2020.Models;
using BugTracker2020.Models.ViewModels;
using BugTracker2020.Services;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using BugTracker2020.Utilities;

namespace BugTracker2020.Controllers
{
  public class ProjectsController : Controller
  {
    private readonly ApplicationDbContext _context;
    private readonly IBTProjectsService _projectService;
    private readonly IBTRolesService _rolesService;
    private readonly UserManager<BTUser> _userManager;

    public ProjectsController(ApplicationDbContext context, IBTProjectsService projectService, IBTRolesService rolesService, UserManager<BTUser> userManager)
    {
      _context = context;
      _projectService = projectService;
      _rolesService = rolesService;
      _userManager = userManager;
    }

    // GET: Projects
    public async Task<IActionResult> Index()
    {
      TempData["InProjectsPage"] = true;
      var projects = await _context.Projects
        .Include(p => p.ProjectUsers)
        .ThenInclude(p => p.User)
        .ToListAsync();
      return View(projects);
    }

    // GET: MyProjects
    public async Task<IActionResult> MyProjects()
    {
      TempData["InProjectsPage"] = true;
      var userId = _userManager.GetUserId(User); // Get the currently logged in user.
      var roleList = await _rolesService.ListUserRoles(_context.Users.Find(userId));
      var role = roleList.FirstOrDefault();
      List<Project> model;
      switch (role)
      {
        case "Admin":
          model = _context.Projects.ToList();
          break;

        // Snippet to get ticket for project manager - special case for roles
        case "ProjectManager":
          var projectIds = new List<int>();
          model = new List<Project>();
          var pmProjects = _context.ProjectUsers.Where(pu => pu.UserId == userId).ToList();

          foreach (var record in pmProjects)
          {
            projectIds.Add(_context.Projects.Find(record.ProjectId).Id);
          }
          foreach (var id in projectIds)
          {
            var projects = _context.Projects.Where(t => t.Id == id).ToList();

            model.AddRange(projects);
          }
          break;

        case "Developer":
          model = new List<Project>();
          var devProjectIds = new List<int>();
          var devProjects = _context.ProjectUsers.Where(pu => pu.UserId == userId).ToList();
          foreach (var record in devProjects)
          {
            devProjectIds.Add(_context.Projects.Find(record.ProjectId).Id);
          }
          foreach (var id in devProjectIds)
          {
            var projects = _context.Projects.Where(p => p.Id == id).ToList();
            model.AddRange(projects);
          }
          break;

        case "Submitter":
        case "NewUser":
          model = new List<Project>();
          var otherProjectIds = new List<int>();
          var otherProjects = _context.ProjectUsers.Where(pu => pu.UserId == userId);
          foreach (var record in otherProjects)
          {
            otherProjectIds.Add(_context.Projects.Find(record.ProjectId).Id);
          }
          foreach (var id in otherProjectIds)
          {
            var projects = _context.Projects.Where(p => p.Id == id).ToList();
            model.AddRange(projects);
          }
          break;

        default:
          return RedirectToAction("Index");
      }

      return View(model);
    }


    // GET: Projects/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      TempData["InProjectsPage"] = true;
      if (id == null)
      {
        return NotFound();
      }

      var vm = new ProjectTicketsViewModel();

      var project = await _context.Projects
        .Include(p => p.ProjectUsers)
        .ThenInclude(pu => pu.User)
        .FirstOrDefaultAsync(m => m.Id == id);
      if (project == null)
      {
        return NotFound();
      }

      var tickets = await _context.Tickets
        .Include(t=>t.TicketPriority)
        .Include(t=>t.TicketStatus)
        .Include(t=>t.TicketType)
        .Include(t=>t.Attachments)
        .Include(t=>t.Histories)
        .Where(t => t.ProjectId == id.Value)
        .ToListAsync();
      vm.Project = project;
      vm.Tickets = tickets;
      return View(vm);
    }

    // GET: Projects/Create
    public IActionResult Create()
    {
      return View();
    }

    // POST: Projects/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name")] string projectName, IFormFile projectImage)
    {
      if (ModelState.IsValid)
      {
        var attachmentHandler = new AttachmentHandler();
        var project = attachmentHandler.AttachToProject(projectImage);
        project.Name = projectName;
        _context.Add(project);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      }
      return View();
    }

    // GET: Projects/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      TempData["InProjectsPage"] = true;
      if (id == null)
      {
        return NotFound();
      }

      var project = await _context.Projects.FindAsync(id);
      if (project == null)
      {
        return NotFound();
      }
      return View(project);
    }

    // POST: Projects/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string projectName, IFormFile projectImage)
    {

      if (ModelState.IsValid)
      {
        try
        {
          var attachmentHandler = new AttachmentHandler();
          var project = attachmentHandler.AttachToProject(projectImage);
          project.Name = projectName;
          project.Id = id;
          _context.Update(project);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!ProjectExists(id))
          {
            return NotFound();
          }
          else
          {
            throw;
          }
        }
        return RedirectToAction(nameof(Index));
      }
      return View();
    }

    // GET: Projects/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var project = await _context.Projects
          .FirstOrDefaultAsync(m => m.Id == id);
      if (project == null)
      {
        return NotFound();
      }

      return View(project);
    }

    // POST: Projects/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var project = await _context.Projects.FindAsync(id);
      _context.Projects.Remove(project);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    private bool ProjectExists(int id)
    {
      return _context.Projects.Any(e => e.Id == id);
    }

    // GET: ManageProjectUsers/projectId
    public async Task<IActionResult> ManageProjectUsers()
    {
      //// Display a list of users
      //var model = new ManageProjectUsersViewModel();
      //// Set vm's Project property to the currently selected project
      //model.Project = _context.Projects.Find(id);
      //// Get a list of users for the selectList
      //List<BTUser> users = await _context.Users.ToListAsync();
      //// Get a list of current members for the selectList
      //List<BTUser> members = (List<BTUser>)await _projectService.UsersOnProject(id);
      //// Set our Users property on the vm
      //model.Users = new MultiSelectList(users, "Id", "FullName", members);
      //// send the vm to the view
      ///

      TempData["InUserAssignmentsPage"] = true;

      // New Version - Display a list of users
      // For each user display a list of projects
      // Assign project to user button

      // Have a viewmodel for each user project assignment
      var model = new List<ManageProjectUsersViewModel>();
      // Get all users
      var users = await _context.Users.ToListAsync();

      foreach (var user in users)
      {
        var vm = new ManageProjectUsersViewModel();
        if (!await _rolesService.IsUserInRole(user, "Demo"))
        {
          vm.User = user;
          vm.UserRole = await _rolesService.ListUserRoles(user);
          vm.CurrentProjects = await _projectService.ListUserProjects(user.Id);
          var projects = await _context.Projects.ToListAsync();
          vm.Projects = new SelectList(projects, "Id", "Name");
          model.Add(vm);
        }
      }



      return View(model);
    }

    // POST: AssignProjectToUser/userId
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignProjectToUser(ManageProjectUsersViewModel item, string userId)
    {
      if (ModelState.IsValid)
      {
        if (item.SelectedProject != 0)
        {
          var user = _context.Users.Find(userId);
          var project = _context.Projects.Find(item.SelectedProject);
          if (await _projectService.IsUserOnProject(user.Id, project.Id))
          {
            // Handle user already assigned instead of error
            TempData["UserAlreadyOn"] = $"{user.FullName} is already assigned to {project.Name}";
            return RedirectToAction("ManageProjectUsers");
          }
          else
          {
            await _projectService.AddUserToProject(user.Id, project.Id);
            return RedirectToAction("ManageProjectUsers");
          }
        }

        // FIGURE OUT WHY MODEL IS ALWAYS NULL
        //if (model.SelectedUsers != null)
        //{
        //  var currentMembers = await _context.Projects
        //    .Include(p => p.ProjectUsers)
        //    .FirstOrDefaultAsync(p => p.Id == model.Project.Id);
        //  List<string> memberIds = currentMembers.ProjectUsers.Select(u => u.UserId).ToList();
        //  //model.SelectedUsers.ToList().ForEach(i => _projectService.RemoveUserFromProject(i, model.Project.Id));

        //  foreach (string id in memberIds)
        //  {
        //    await _projectService.RemoveUserFromProject(id, model.Project.Id);
        //  }

        //  foreach (string id in model.SelectedUsers)
        //  {
        //    await _projectService.AddUserToProject(id, model.Project.Id);
        //  }

        //  return RedirectToAction("Index", "Projects");
        //}
        //else
        //{
        //  // send error
        //}
      }
      return RedirectToAction("Index");
    }

    // GET: RemoveAssignedProjectFromUser/projectId
    public async Task<IActionResult> RemoveAssignedProjectFromUser(int id, string userId)
    {
      if (ModelState.IsValid)
      {

        var user = _context.Users.Find(userId);
        var project = _context.Projects.Find(id);

        await _projectService.RemoveUserFromProject(user.Id, project.Id);
        return RedirectToAction("ManageProjectUsers");

        //if (model.SelectedUsers != null)
        //{
        //  var currentMembers = await _context.Projects
        //    .Include(p => p.ProjectUsers)
        //    .FirstOrDefaultAsync(p => p.Id == model.Project.Id);
        //  List<string> memberIds = currentMembers.ProjectUsers.Select(u => u.UserId).ToList();
        //  //model.SelectedUsers.ToList().ForEach(i => _projectService.RemoveUserFromProject(i, model.Project.Id));

        //  foreach (string id in memberIds)
        //  {
        //    await _projectService.RemoveUserFromProject(id, model.Project.Id);
        //  }

        //  foreach (string id in model.SelectedUsers)
        //  {
        //    await _projectService.AddUserToProject(id, model.Project.Id);
        //  }

        //  return RedirectToAction("Index", "Projects");
        //}
        //else
        //{
        //  // send error
        //}
      }
      return NotFound();
    }
  }
}
