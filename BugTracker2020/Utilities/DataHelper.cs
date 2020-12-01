﻿using BugTracker2020.Data;
using BugTracker2020.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using System;
using System.Threading.Tasks;

namespace BugTracker2020.Utilities
{
  public static class DataHelper
  {
    public static string GetConnectionString(IConfiguration configuration)
    {
      // The default connection string will come from appSettings like usual
      var connectionString = configuration.GetConnectionString("DefaultConnection");

      // It will be automatically overwritten if we are running on Heroku
      var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
      return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
    }

    public static string BuildConnectionString(string databaseUrl)
    {
      // Provides an object representation of a uniform resource identifier (URI) and easy access
      // to the parts of the URI.

      var databaseUri = new Uri(databaseUrl);
      var userInfo = databaseUri.UserInfo.Split(':');

      // Provides a simple way to create and manage the contents of connection strings
      // used by the NpgsqlConnection class

      var builder = new NpgsqlConnectionStringBuilder
      {
        Host = databaseUri.Host,
        Port = databaseUri.Port,
        Username = userInfo[0],
        Password = userInfo[1],
        Database = databaseUri.LocalPath.TrimStart('/')
      };
      return builder.ToString();
    }
    public static async Task ManageData(IHost host)
    {
      try
      {
        // This technique is used to obtain references to services
        // Normally I would just inject these services but you can't use a constructor in
        // A static class...
        using var svcScope = host.Services.CreateScope();
        var svcProvider = svcScope.ServiceProvider;

        // The service will run your migrations
        var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
        await dbContextSvc.Database.MigrateAsync();

        // Seed Data
        var services = svcScope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationDbContext>();
        var usermanager = services.GetRequiredService<UserManager<BTUser>>();
        var rolemanager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await ContextSeed.SeedRolesAsync(rolemanager);
        await ContextSeed.SeedDefaultUsersAsync(usermanager);
        await ContextSeed.SeedDefaultTicketPropsAsync(context);
        await ContextSeed.SeedProjectsAsync(context);
        await ContextSeed.SeedProjectUsersAsync(context, usermanager);
        await ContextSeed.SeedTicketsAsync(context, usermanager);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Exception while running Manage Data -> {ex}");
      }
    }
  }
}
