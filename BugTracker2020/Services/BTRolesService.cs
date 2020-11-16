using BugTracker2020.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker2020.Services
{
  public class BTRolesService : IBTRolesService
  {
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<BTUser> _userManager;

    public BTRolesService(RoleManager<IdentityRole> roleManager, UserManager<BTUser> userManager)
    {
      _roleManager = roleManager;
      _userManager = userManager;
    }
    public async Task<bool> AddUserToRole(BTUser user, string roleName)
    {
      var result = await _userManager.AddToRoleAsync(user, roleName);
      return result.Succeeded;
    }

    public async Task<bool> IsUserInRole(BTUser user, string roleName)
    {
      return await _userManager.IsInRoleAsync(user, roleName);
    }

    public async Task<IEnumerable<string>> ListUserRoles(BTUser user)
    {
      return await _userManager.GetRolesAsync(user);
    }

    public async Task<bool> RemoveUserFromRole(BTUser user, string roleName)
    {
      var result = await _userManager.RemoveFromRoleAsync(user, roleName);
      return result.Succeeded;
    }

    public async Task<ICollection<BTUser>> UsersInRole(string roleName)
    {
      return await _userManager.GetUsersInRoleAsync(roleName);
    }

    public async Task<ICollection<BTUser>> UsersNotInRole(IdentityRole role)
    {
      var roleId = await _roleManager.GetRoleIdAsync(role);
      // Get the userManager service
      // Get all users with .Users
      // Where (some condition is met)
      // What is a user?
      // [user1, user2, user3, user4]
      // u = one of the users
      // IsUserInRole is a method that requires two arguments, a user and a role name and returns true or false
      // .Result 
      return _userManager.Users.Where(u => IsUserInRole(u, role.Name).Result == false).ToList();
    }

    
  }
}
