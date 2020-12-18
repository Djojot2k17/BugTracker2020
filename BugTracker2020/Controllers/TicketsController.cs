using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracker2020.Data;
using BugTracker2020.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using BugTracker2020.Utilities;
using BugTracker2020.Services;
using BugTracker2020.Models.ViewModels;

namespace BugTracker2020.Controllers
{
  public class TicketsController : Controller
  {
    private readonly ApplicationDbContext _context;
    private readonly UserManager<BTUser> _userManager;
    private readonly IBTHistoriesService _historyService;
    private readonly IBTAccessService _accessService;
    private readonly IBTRolesService _rolesService;

    public TicketsController(ApplicationDbContext context, UserManager<BTUser> userManager, IBTHistoriesService historyService, IBTAccessService accessService, IBTRolesService rolesService)
    {
      _context = context;
      _userManager = userManager;
      _historyService = historyService;
      _accessService = accessService;
      _rolesService = rolesService;
    }

    // GET: Tickets
    public async Task<IActionResult> Index()
    {
      TempData["InTicketsPage"] = true;
      var applicationDbContext = _context.Tickets.Include(t => t.DeveloperUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).Include(t => t.Attachments);
      return View(await applicationDbContext.ToListAsync());
    }

    //GET: MyTickets
    public async Task<IActionResult> MyTickets()
    {
      TempData["InTicketsPage"] = true;
      var userId = _userManager.GetUserId(User); // Get the currently logged in user.
      var roleList = await _rolesService.ListUserRoles(_context.Users.Find(userId));
      var role = roleList.FirstOrDefault();
      List<Ticket> model;
      switch (role)
      {
        case "Admin":
          model = _context.Tickets
              .Include(t => t.OwnerUser)
              .Include(t => t.TicketPriority)
              .Include(t => t.TicketStatus)
              .Include(t => t.TicketType)
              .Include(t => t.Project)
              .ToList();
          break;

        // Snippet to get ticket for project manager - special case for roles
        case "ProjectManager":
          var projectIds = new List<int>();
          model = new List<Ticket>();
          var userProjects = _context.ProjectUsers.Where(pu => pu.UserId == userId).ToList();

          foreach (var record in userProjects)
          {
            projectIds.Add(_context.Projects.Find(record.ProjectId).Id);
          }
          foreach (var id in projectIds)
          {
            var tickets = _context.Tickets.Where(t => t.ProjectId == id)
                .Include(t => t.OwnerUser)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .Include(t => t.Project)
                .ToList();

            model.AddRange(tickets);
          }
          break;

        case "Developer":
          model = _context.Tickets.Where(t => t.DeveloperUserId == userId)
              .Include(t => t.OwnerUser)
              .Include(t => t.TicketPriority)
              .Include(t => t.TicketStatus)
              .Include(t => t.TicketType)
              .Include(t => t.Project)
              .ToList();
          break;

        case "Submitter":
        case "NewUser":
          model = _context.Tickets.Where(t => t.OwnerUserId == userId)
              .Include(t => t.OwnerUser)
              .Include(t => t.TicketPriority)
              .Include(t => t.TicketStatus)
              .Include(t => t.TicketType)
              .Include(t => t.Project)
              .ToList();
          break;

        default:
          return RedirectToAction("Index");
      }

      return View(model);
    }

    // GET: Tickets/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      TempData["InTicketsPage"] = true;
      if (id == null)
      {
        return NotFound();
      }
      var vm = new TicketDetailsViewModel();
      var ticket = await _context.Tickets
          .Include(t => t.DeveloperUser)
          .Include(t => t.OwnerUser)
          .Include(t => t.Project)
          .Include(t => t.TicketPriority)
          .Include(t => t.TicketStatus)
          .Include(t => t.TicketType)
          .Include(t => t.Attachments)
          .Include(t => t.Comments)
          .FirstOrDefaultAsync(m => m.Id == id);
      if (ticket == null)
      {
        return NotFound();
      }
      vm.Ticket = ticket;

      return View(vm);
    }

    // GET: Tickets/Create
    public async Task<IActionResult> Create(int? id)
    {
      TempData["InTicketsPage"] = true;
      if (id != null)
      {
        // Instanciate a new ticket and set the project id there
        ViewData["ProjectIdGET"] = id.Value;
        var model = new Ticket();
        model.ProjectId = id.Value;
        model.Project = _context.Projects.Find(id.Value);
        var users = _context.Users.ToList();
        var developers = new List<BTUser>();
        foreach (var developer in users)
        {
          if (await _rolesService.IsUserInRole(developer, "Developer"))
          {
            developers.Add(developer);
          }
        }
        if (User.IsInRole("Admin") || User.IsInRole("ProjectManager"))
        {
          ViewData["DeveloperUserId"] = new SelectList(developers, "Id", "FullName");
          ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name");
          ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name");
          ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name");
        }
        if (User.IsInRole("Developer"))
        {
          ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name");
          ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name");
          ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name");
        }
        
        return View(model);
      }
      if (User.IsInRole("Submitter"))
      {
        ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
        ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name");
        ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "FullName");
      }
      return View();
    }

    // POST: Tickets/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Description,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,DeveloperUserId")] Ticket ticket, List<IFormFile> attachments)
    {
      if (ModelState.IsValid)
      {
        // IF not demo user
        if (!User.IsInRole("Demo"))
        {
          // Add file handler
          ticket.OwnerUserId = _userManager.GetUserId(User);
          ticket.Created = DateTime.Now;
          if (attachments != null)
          {
            foreach (var attachment in attachments)
            {
              AttachmentHandler attachmentHandler = new AttachmentHandler();
              ticket.Attachments.Add(attachmentHandler.AttachToTicket(attachment, ticket.Id));
            }
          }
          if (User.IsInRole("Submitter"))
          {
            ticket.TicketTypeId = 1;
            ticket.TicketStatusId = 1;
          }
          _context.Add(ticket);
          await _context.SaveChangesAsync();
          return RedirectToAction(nameof(Index));
        }
        else
        {
          // Handle tempdata["DemoLockout"]
          TempData["DemoLockout"] = "Your changes have not been saved. You must be logged in as a full user.";
          //Handle redirect to index
          return RedirectToAction(nameof(Index));
        }
      }
      ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.DeveloperUserId);
      ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.OwnerUserId);
      ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
      ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Id", ticket.TicketPriorityId);
      ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Id", ticket.TicketStatusId);
      ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Id", ticket.TicketTypeId);
      return View(ticket);
    }

    // GET: Tickets/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }
      var userId = _userManager.GetUserId(User);
      var roleName = (await _userManager.GetRolesAsync(await _userManager.GetUserAsync(User))).FirstOrDefault();
      // If you have access to a ticket
      if (await _accessService.CanAccessTicket(userId, (int)id, roleName))
      {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null)
        {
          return NotFound();
        }
        var users = _context.Users.ToList();
        var developers = new List<BTUser>();
        foreach(var developer in users)
        {
          if(await _rolesService.IsUserInRole(developer, "Developer"))
          {
            developers.Add(developer);
          }
        }
        if (User.IsInRole("Developer"))
        {
          ticket.DeveloperUserId = userId;
        }
        if (User.IsInRole("Submitter"))
        {
          ticket.DeveloperUserId = _context.Tickets.Find(id.Value).DeveloperUserId;
        }
        ViewData["DeveloperUserId"] = new SelectList(developers, "Id", "FullName", ticket.DeveloperUserId);
        //ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.OwnerUserId);
        //ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
        ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
        ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
        ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name", ticket.TicketTypeId);
        return View(ticket);
      }
      TempData["Naughty"] = "You've been a baaad baaaad boy...";
      return RedirectToAction("Index");
    }

    // POST: Tickets/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Created,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,DeveloperUserId")] Ticket ticket)
    {
      if (id != ticket.Id)
      {
        return NotFound();
      }
      // Get snapshot of old ticket
      Ticket oldTicket = await _context.Tickets
        .Include(t => t.TicketPriority)
        .Include(t => t.TicketStatus)
        .Include(t => t.TicketType)
        .Include(t => t.DeveloperUser)
        .Include(t => t.Project)
        .Include(t => t.OwnerUser)
        .AsNoTracking()
        .FirstOrDefaultAsync(t => t.Id == ticket.Id);

      if (ModelState.IsValid)
      {
        try
        {

          ticket.Updated = DateTime.Now;
          _context.Update(ticket);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!TicketExists(ticket.Id))
          {
            return NotFound();
          }
          else
          {
            throw;
          }
        }
        // Get snapshot of new ticket
        Ticket newTicket = await _context.Tickets
          .Include(t => t.TicketPriority)
          .Include(t => t.TicketStatus)
          .Include(t => t.TicketType)
          .Include(t => t.DeveloperUser)
          .Include(t => t.Project)
          .Include(t => t.OwnerUser)
          .AsNoTracking()
          .FirstOrDefaultAsync(t => t.Id == ticket.Id);
        // Get Current UserId
        string userId = _userManager.GetUserId(User);
        // Add History
        await _historyService.AddHistory(oldTicket, newTicket, userId);
        return RedirectToAction(nameof(Index));
      }
      ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.DeveloperUserId);
      ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "Id", ticket.OwnerUserId);
      ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
      ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Id", ticket.TicketPriorityId);
      ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Id", ticket.TicketStatusId);
      ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Id", ticket.TicketTypeId);
      return View(ticket);
    }

    // GET: Tickets/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var ticket = await _context.Tickets
          .Include(t => t.DeveloperUser)
          .Include(t => t.OwnerUser)
          .Include(t => t.Project)
          .Include(t => t.TicketPriority)
          .Include(t => t.TicketStatus)
          .Include(t => t.TicketType)
          .FirstOrDefaultAsync(m => m.Id == id);
      if (ticket == null)
      {
        return NotFound();
      }

      return View(ticket);
    }

    // POST: Tickets/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var ticket = await _context.Tickets.FindAsync(id);
      _context.Tickets.Remove(ticket);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    private bool TicketExists(int id)
    {
      return _context.Tickets.Any(e => e.Id == id);
    }

    public async Task<FileResult> DownloadFile(int? id)
    {
      TicketAttachment attachment = await _context.TicketAttachments.FindAsync(id.Value);
      return File(attachment.FileData,attachment.ContentType);
    }
  }
}
