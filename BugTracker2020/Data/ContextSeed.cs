﻿using BugTracker2020.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker2020.Data
{
  public enum Roles
  {
    Admin,
    ProjectManager,
    Developer,
    Submitter,
    NewUser,
    Demo
  }
  public static class ContextSeed
  {
    // Seed Roles
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
      await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
      await roleManager.CreateAsync(new IdentityRole(Roles.ProjectManager.ToString()));
      await roleManager.CreateAsync(new IdentityRole(Roles.Developer.ToString()));
      await roleManager.CreateAsync(new IdentityRole(Roles.Submitter.ToString()));
      await roleManager.CreateAsync(new IdentityRole(Roles.NewUser.ToString()));
      await roleManager.CreateAsync(new IdentityRole(Roles.Demo.ToString()));

    }

    // Seed Users
    public static async Task SeedDefaultUsersAsync(UserManager<BTUser> userManager)
    {
      #region AdminUser
      //Create our userObject
      var defaultAdmin = new BTUser
      {
        Email = "exampleAdmin@code.com",
        UserName = "exampleAdmin@code.com",
        FirstName = "Admin",
        LastName = "Istrator",
        EmailConfirmed = true,
        PhoneNumberConfirmed = true
      };

      try
      {
        var user = await userManager.FindByEmailAsync(defaultAdmin.Email);
        if (user == null)
        {
          //userManager.CreateAsync(userObject, password)
          await userManager.CreateAsync(defaultAdmin, "Abc123!@#");
          await userManager.AddToRoleAsync(defaultAdmin, Roles.Admin.ToString());
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine("********** ERROR *********");
        Debug.WriteLine("Error Seeding Default Admin User");
        Debug.WriteLine(ex.Message);
        Debug.WriteLine("***************************");
        throw;
      }
      #endregion

      #region ProjectManagerUser
      //Create our userObject
      var projectManager = new BTUser
      {
        Email = "examplePm@code.com",
        UserName = "examplePm@code.com",
        FirstName = "Project",
        LastName = "Manager",
        EmailConfirmed = true,
        PhoneNumberConfirmed = true
      };

      try
      {
        var user = await userManager.FindByEmailAsync(projectManager.Email);
        if (user == null)
        {
          //userManager.CreateAsync(userObject, password)
          await userManager.CreateAsync(projectManager, "Abc123!@#");
          await userManager.AddToRoleAsync(projectManager, Roles.ProjectManager.ToString());
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine("********** ERROR *********");
        Debug.WriteLine("Error Seeding Default PM User");
        Debug.WriteLine(ex.Message);
        Debug.WriteLine("***************************");
        throw;
      }
      #endregion

      #region DeveloperUser
      //Create our userObject
      var developer = new BTUser
      {
        Email = "exampleDev@code.com",
        UserName = "exampleDev@code.com",
        FirstName = "Dev",
        LastName = "Eloper",
        EmailConfirmed = true,
        PhoneNumberConfirmed = true
      };

      try
      {
        var user = await userManager.FindByEmailAsync(developer.Email);
        if (user == null)
        {
          //userManager.CreateAsync(userObject, password)
          await userManager.CreateAsync(developer, "Abc123!@#");
          await userManager.AddToRoleAsync(developer, Roles.Developer.ToString());
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine("********** ERROR *********");
        Debug.WriteLine("Error Seeding Default Dev User");
        Debug.WriteLine(ex.Message);
        Debug.WriteLine("***************************");
        throw;
      }
      #endregion

      #region Submitter
      //Create our userObject
      var submitter = new BTUser
      {
        Email = "exampleSubmitter@code.com",
        UserName = "exampleSubmitter@code.com",
        FirstName = "Sub",
        LastName = "Human",
        EmailConfirmed = true,
        PhoneNumberConfirmed = true
      };

      try
      {
        var user = await userManager.FindByEmailAsync(submitter.Email);
        if (user == null)
        {
          //userManager.CreateAsync(userObject, password)
          await userManager.CreateAsync(submitter, "Abc123!@#");
          await userManager.AddToRoleAsync(submitter, Roles.Submitter.ToString());
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine("********** ERROR *********");
        Debug.WriteLine("Error Seeding Default Submitter User");
        Debug.WriteLine(ex.Message);
        Debug.WriteLine("***************************");
        throw;
      }
      #endregion

      #region NewUser
      //Create our userObject
      var newUser = new BTUser
      {
        Email = "exampleNoob@code.com",
        UserName = "exampleNoob@code.com",
        FirstName = "Noob",
        LastName = "User",
        EmailConfirmed = true,
        PhoneNumberConfirmed = true
      };

      try
      {
        var user = await userManager.FindByEmailAsync(newUser.Email);
        if (user == null)
        {
          //userManager.CreateAsync(userObject, password)
          await userManager.CreateAsync(newUser, "Abc123!@#");
          await userManager.AddToRoleAsync(newUser, Roles.NewUser.ToString());
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine("********** ERROR *********");
        Debug.WriteLine("Error Seeding Default NewUser User");
        Debug.WriteLine(ex.Message);
        Debug.WriteLine("***************************");
        throw;
      }
      #endregion

      #region Demo AdminUser
      //Create our userObject
      var demoAdmin = new BTUser
      {
        Email = "demoAdmin@mailinator.com",
        UserName = "demoAdmin@mailinator.com",
        FirstName = "Demo",
        LastName = "Admin",
        EmailConfirmed = true,
        PhoneNumberConfirmed = true
      };

      try
      {
        var user = await userManager.FindByEmailAsync(demoAdmin.Email);
        if (user == null)
        {
          //userManager.CreateAsync(userObject, password)
          await userManager.CreateAsync(demoAdmin, "Abc123!@#");
          await userManager.AddToRoleAsync(demoAdmin, Roles.Admin.ToString());
          await userManager.AddToRoleAsync(demoAdmin, Roles.Demo.ToString());

        }

      }
      catch (Exception ex)
      {
        Debug.WriteLine("********** ERROR *********");
        Debug.WriteLine("Error Seeding Default Admin User");
        Debug.WriteLine(ex.Message);
        Debug.WriteLine("***************************");
        throw;
      }
      #endregion

      #region DemoProjectManagerUser
      //Create our userObject
      var demoPm = new BTUser
      {
        Email = "demoPm@mailinator.com",
        UserName = "demoPm@mailinator.com",
        FirstName = "Demo",
        LastName = "PM",
        EmailConfirmed = true,
        PhoneNumberConfirmed = true
      };

      try
      {
        var user = await userManager.FindByEmailAsync(demoPm.Email);
        if (user == null)
        {
          //userManager.CreateAsync(userObject, password)
          await userManager.CreateAsync(demoPm, "Abc123!@#");
          await userManager.AddToRoleAsync(demoPm, Roles.ProjectManager.ToString());
          await userManager.AddToRoleAsync(demoPm, Roles.Demo.ToString());

        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine("********** ERROR *********");
        Debug.WriteLine("Error Seeding Default PM User");
        Debug.WriteLine(ex.Message);
        Debug.WriteLine("***************************");
        throw;
      }
      #endregion

      #region DemoDeveloperUser
      //Create our userObject
      var demoDeveloper = new BTUser
      {
        Email = "demoDev@mailinator.com",
        UserName = "demoDev@mailinator.com",
        FirstName = "Demo",
        LastName = "Dev",
        EmailConfirmed = true,
        PhoneNumberConfirmed = true
      };

      try
      {
        var user = await userManager.FindByEmailAsync(demoDeveloper.Email);
        if (user == null)
        {
          //userManager.CreateAsync(userObject, password)
          await userManager.CreateAsync(demoDeveloper, "Abc123!@#");
          await userManager.AddToRoleAsync(demoDeveloper, Roles.Developer.ToString());
          await userManager.AddToRoleAsync(demoDeveloper, Roles.Demo.ToString());

        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine("********** ERROR *********");
        Debug.WriteLine("Error Seeding Default Dev User");
        Debug.WriteLine(ex.Message);
        Debug.WriteLine("***************************");
        throw;
      }
      #endregion

      #region DemoSubmitter
      //Create our userObject
      var demoSubmitter = new BTUser
      {
        Email = "demoSub@mailinator.com",
        UserName = "demoSub@mailinator.com",
        FirstName = "Demo",
        LastName = "Submitter",
        EmailConfirmed = true,
        PhoneNumberConfirmed = true
      };

      try
      {
        var user = await userManager.FindByEmailAsync(demoSubmitter.Email);
        if (user == null)
        {
          //userManager.CreateAsync(userObject, password)
          await userManager.CreateAsync(demoSubmitter, "Abc123!@#");
          await userManager.AddToRoleAsync(demoSubmitter, Roles.Submitter.ToString());
          await userManager.AddToRoleAsync(demoSubmitter, Roles.Demo.ToString());

        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine("********** ERROR *********");
        Debug.WriteLine("Error Seeding Default Submitter User");
        Debug.WriteLine(ex.Message);
        Debug.WriteLine("***************************");
        throw;
      }
      #endregion

      #region DemoNewUser
      //Create our userObject
      var demoNewUser = new BTUser
      {
        Email = "demoNoobie@mailinator.com",
        UserName = "demoNoobie@mailinator.com",
        FirstName = "Demo",
        LastName = "Noob",
        EmailConfirmed = true,
        PhoneNumberConfirmed = true
      };

      try
      {
        var user = await userManager.FindByEmailAsync(demoNewUser.Email);
        if (user == null)
        {
          //userManager.CreateAsync(userObject, password)
          await userManager.CreateAsync(demoNewUser, "Abc123!@#");
          await userManager.AddToRoleAsync(demoNewUser, Roles.NewUser.ToString());
          await userManager.AddToRoleAsync(demoNewUser, Roles.Demo.ToString());

        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine("********** ERROR *********");
        Debug.WriteLine("Error Seeding Default NewUser User");
        Debug.WriteLine(ex.Message);
        Debug.WriteLine("***************************");
        throw;
      }
      #endregion
    }

    // Seed ticket info
    public static async Task SeedDefaultTicketPropsAsync(ApplicationDbContext context)
    {
      try
      {
        // Priorities
        if (!context.TicketPriorities.Any(t => t.Name == "Unassigned"))
        {
          await context.TicketPriorities.AddAsync(new TicketPriority { Name = "Unassigned" });
        }
        if (!context.TicketPriorities.Any(t => t.Name == "Low"))
        {
          await context.TicketPriorities.AddAsync(new TicketPriority { Name = "Low" });
        }
        if (!context.TicketPriorities.Any(t => t.Name == "Medium"))
        {
          await context.TicketPriorities.AddAsync(new TicketPriority { Name = "Medium" });
        }
        if (!context.TicketPriorities.Any(t => t.Name == "High"))
        {
          await context.TicketPriorities.AddAsync(new TicketPriority { Name = "High" });
        }

        // Status
        if (!context.TicketStatuses.Any(t => t.Name == "Unassigned"))
        {
          await context.TicketStatuses.AddAsync(new TicketStatus { Name = "Unassigned" });
        }
        if (!context.TicketStatuses.Any(t => t.Name == "Open"))
        {
          await context.TicketStatuses.AddAsync(new TicketStatus { Name = "Open" });
        }
        if (!context.TicketStatuses.Any(t => t.Name == "InProgress"))
        {
          await context.TicketStatuses.AddAsync(new TicketStatus { Name = "InProgress" });
        }
        if (!context.TicketStatuses.Any(t => t.Name == "Closed"))
        {
          await context.TicketStatuses.AddAsync(new TicketStatus { Name = "Closed" });
        }

        // Type
        if (!context.TicketTypes.Any(t => t.Name == "Unassigned"))
        {
          await context.TicketTypes.AddAsync(new TicketType { Name = "Unassigned" });
        }
        if (!context.TicketTypes.Any(t => t.Name == "BackEnd"))
        {
          await context.TicketTypes.AddAsync(new TicketType { Name = "BackEnd" });
        }
        if (!context.TicketTypes.Any(t => t.Name == "FrontEnd"))
        {
          await context.TicketTypes.AddAsync(new TicketType { Name = "FrontEnd" });
        }
        if (!context.TicketTypes.Any(t => t.Name == "DataBase"))
        {
          await context.TicketTypes.AddAsync(new TicketType { Name = "DataBase" });
        }

        await context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        Debug.WriteLine($"Error something went wrong... {ex}");
      }
    }
  }
}
