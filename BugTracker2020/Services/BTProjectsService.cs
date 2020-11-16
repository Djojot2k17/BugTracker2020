using BugTracker2020.Data;
using BugTracker2020.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker2020.Services
{
  public class BTProjectsService : IBTProjectsService
  {
    // Local variable to inject applicationdbcontext
    private readonly ApplicationDbContext _context;

    // Constructor
    public BTProjectsService(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task AddUserToProject(string userId, int projectId)
    {
      if (!await IsUserOnProject(userId, projectId))
      {
        try
        {
          await _context.ProjectUsers.AddAsync(new ProjectUser { ProjectId = projectId, UserId = userId });
          await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
          Debug.WriteLine($"Error adding user to project -> {ex.Message}");
          throw;
        }
      }
    }

    public async Task<bool> IsUserOnProject(string userId, int projectId)
    {
      // ProjectUsers Table
      // UserId(guid) , ProjectId(int)

      // Set up project variable to check whether a given user is part of a given project

      // Get the Projects table from the database
      Project project = await _context.Projects
          // Include rows of the ProjectUsers table where the row's userId matches the given userId
          .Include(u => u.ProjectUsers)
          // Also include rows of project objects (that match the previous query)
          .ThenInclude(u => u.User)
          // BTUser User is included in the projectUser model
          // So we can use the model to find User.Id
          // And select the first project where the project.Id matches the given projectId 
          .FirstOrDefaultAsync(p => p.Id == projectId);

      // IF found return true, else false
      bool result = project.ProjectUsers.Any(u => u.UserId == userId);

      return result;
      //return _context.ProjectUsers.Where(pu => pu.UserId == userId && pu.ProjectId == projectId).Any();
    }

    public async Task<ICollection<Project>> ListUserProjects(string userId)
    {
      // Build an intermediate table and store it as user
      // Get the users table from the database
      BTUser user = await _context.Users
          // Include the rows of the ProjectUsers table
          .Include(p => p.ProjectUsers)
          // Also include the project objects
          .ThenInclude(p => p.Project)
          // And get the first row where the identity user id matches the given userId
          .FirstOrDefaultAsync(u => u.Id == userId); // Ends query

      List<Project> projects = user.ProjectUsers
        .Select(p => p.Project)
        .ToList();
      return projects;
    }

    public async Task RemoveUserFromProject(string userId, int projectId)
    {
      if (await IsUserOnProject(userId, projectId))
      {
        try
        {
          ProjectUser projectUser = _context.ProjectUsers.Where(m => m.UserId == userId && m.ProjectId == projectId).FirstOrDefault();
          
          // Why is this tracking the same projectUser kv pairs?
          _context.ProjectUsers.Remove(projectUser);
          await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
          Debug.WriteLine($"Error, could not remove a user from the project -> {ex.Message}");
          throw;
        }
      }
    }

    public async Task<ICollection<BTUser>> UsersNotOnProject(int projectId)
    {
      return await _context.Users.Where(u => IsUserOnProject(u.Id, projectId).Result == false).ToListAsync();
    }

    public async Task<ICollection<BTUser>> UsersOnProject(int projectId)
    {
      Project project = await _context.Projects
        .Include(u => u.ProjectUsers)
        .ThenInclude(pu => pu.User)
        .FirstOrDefaultAsync(u => u.Id == projectId);
      List<BTUser> projectUsers = project.ProjectUsers.Select(p => p.User).ToList();
      return projectUsers;
    }
  }
}
