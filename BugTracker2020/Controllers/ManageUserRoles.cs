using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker2020.Data;
using BugTracker2020.Models;
using BugTracker2020.Models.ViewModels;
using BugTracker2020.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTracker2020.Controllers
{
  public class ManageUserRoles : Controller
  {
    private readonly ApplicationDbContext _context;
    private readonly UserManager<BTUser> _userManager;
    private readonly IBTRolesService _BTRoles;
    public ManageUserRoles(ApplicationDbContext context, UserManager<BTUser> userManager, IBTRolesService btRoles)
    {
      _context = context;
      _userManager = userManager;
      _BTRoles = btRoles;
    }
    public IActionResult Index()
    {
      return View();
    }

    // GET: ManageUserRoles
    public async Task<IActionResult> SetRoles()
    {

      //List<ManageUserRolesViewModel> model = new List<ManageUserRolesViewModel>();
      //List<BTUser> users = _context.Users.ToList();

      //foreach(var user in users)
      //{
      //  ManageUserRolesViewModel vm = new ManageUserRolesViewModel();
      //  vm.User = user;
      //  var selected = await _BTRoles.ListUserRoles(user);
      //  vm.Roles = new MultiSelectList(_context.Roles, "Name", "Name", selected);
      //  model.Add(vm);
      //}

      List<ManageUserRolesViewModel> model = new List<ManageUserRolesViewModel>();
      List<BTUser> users = _context.Users.ToList();
      
      foreach(var user in users)
      {
        ManageUserRolesViewModel vm = new ManageUserRolesViewModel();
        vm.User = user;
        var selected = await _BTRoles.ListUserRoles(user);
        vm.Roles = new SelectList(_context.Roles, "Name", "Name", selected);
        vm.UserRole = await _BTRoles.ListUserRoles(user);
        model.Add(vm);
      }

      return View(model);
    }

    // POST: SetRoles
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetRoles(ManageUserRolesViewModel item)
    {
      BTUser user = _context.Users.Find(item.User.Id);

      IEnumerable<string> roles = await _BTRoles.ListUserRoles(user);
      await _userManager.RemoveFromRolesAsync(user, roles);
      string userRole = item.SelectedRole;

      if (Enum.TryParse(userRole, out Roles roleValue))
      {
        await _BTRoles.AddUserToRole(user, userRole);
        return RedirectToAction("SetRoles");
      }
      return RedirectToAction("SetRoles");
    }
  }
}
