using BugTracker2020.Data;
using BugTracker2020.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BugTracker2020.Services
{
  public class BTHistoriesService : IBTHistoriesService
  {
    private readonly ApplicationDbContext _context;
    private readonly UserManager<BTUser> _userManager;
    private readonly IEmailSender _emailSender;

    public BTHistoriesService(ApplicationDbContext context, UserManager<BTUser> userManager, IEmailSender emailSender)
    {
      _context = context;
      _userManager = userManager;
      _emailSender = emailSender;
    }

    public async Task AddHistory(Ticket oldTicket, Ticket newTicket, string userId)
    {
      // If oldTicket.prop != newTicket.prop { addHistory }
      if (oldTicket.Title != newTicket.Title)
      {
        TicketHistory history = new TicketHistory
        {
          TicketId = newTicket.Id,
          Property = "Title",
          OldValue = oldTicket.Title,
          NewValue = newTicket.Title,
          Created = DateTimeOffset.Now,
          UserId = userId
        };
        await _context.TicketHistories.AddAsync(history);
      }

      if (oldTicket.Description != newTicket.Description)
      {
        TicketHistory history = new TicketHistory
        {
          TicketId = newTicket.Id,
          Property = "Description",
          OldValue = oldTicket.Description,
          NewValue = newTicket.Description,
          Created = DateTimeOffset.Now,
          UserId = userId
        };
        await _context.TicketHistories.AddAsync(history);
      }

      if (oldTicket.TicketTypeId != newTicket.TicketTypeId)
      {
        TicketHistory history = new TicketHistory
        {
          TicketId = newTicket.Id,
          Property = "Ticket Type",
          OldValue = oldTicket.TicketType.Name,
          NewValue = newTicket.TicketType.Name,
          Created = DateTimeOffset.Now,
          UserId = userId
        };
        await _context.TicketHistories.AddAsync(history);
      }

      if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
      {
        TicketHistory history = new TicketHistory
        {
          TicketId = newTicket.Id,
          Property = "Ticket Priority",
          OldValue = oldTicket.TicketPriority.Name,
          NewValue = newTicket.TicketPriority.Name,
          Created = DateTimeOffset.Now,
          UserId = userId
        };
        await _context.TicketHistories.AddAsync(history);
      }

      if (oldTicket.TicketStatusId != newTicket.TicketStatusId)
      {
        TicketHistory history = new TicketHistory
        {
          TicketId = newTicket.Id,
          Property = "Ticket Status",
          OldValue = oldTicket.TicketStatus.Name,
          NewValue = newTicket.TicketStatus.Name,
          Created = DateTimeOffset.Now,
          UserId = userId
        };
        await _context.TicketHistories.AddAsync(history);
      }

      if (oldTicket.DeveloperUserId != newTicket.DeveloperUserId)
      {
        if (String.IsNullOrWhiteSpace(oldTicket.DeveloperUserId))
        {
          TicketHistory history = new TicketHistory
          {
            TicketId = newTicket.Id,
            Property = "Developer",
            OldValue = "No Developer Assigned",
            NewValue = newTicket.DeveloperUser.FullName,
            Created = DateTimeOffset.Now,
            UserId = userId
          };
          await _context.TicketHistories.AddAsync(history);
        }
        else if (String.IsNullOrWhiteSpace(newTicket.DeveloperUserId))
        {
          TicketHistory history = new TicketHistory
          {
            TicketId = newTicket.Id,
            Property = "Developer",
            OldValue = oldTicket.DeveloperUser.FullName,
            NewValue = "No Developer Assigned",
            Created = DateTimeOffset.Now,
            UserId = userId
          };
          await _context.TicketHistories.AddAsync(history);
        }
        else
        {
          TicketHistory history = new TicketHistory
          {
            TicketId = newTicket.Id,
            Property = "Developer",
            OldValue = oldTicket.DeveloperUser.FullName,
            NewValue = newTicket.DeveloperUser.FullName,
            Created = DateTimeOffset.Now,
            UserId = userId
          };
          await _context.TicketHistories.AddAsync(history);
        }
        //For now -maybe we can get a notification service
        Notification notification = new Notification
        {
          TicketId = newTicket.Id,
          Description = "Time to work mf!",
          Created = DateTime.Now,
          SenderId = userId,
          RecipientId = newTicket.DeveloperUserId
        };
        await _context.Notifications.AddAsync(notification);
        try
        {
          // Send email
          string devEmail = newTicket.DeveloperUser.Email;
          string subject = "New Ticket Assignment";
          string message = $"You have a new ticket for project: {newTicket.Project.Name}";
          await _emailSender.SendEmailAsync(devEmail, subject, message);
        }
        catch (Exception ex)
        {
          Debug.WriteLine($"Couldn't send an email... {ex}");
        }
      }
      await _context.SaveChangesAsync();
    }

    //public async Task AddHistory(Ticket oldTicket, Ticket newTicket, string userId)
    //{
    //  string[] ignoreValues =
    //  {
    //    "Comments",
    //    "Attachments",
    //    "Histories",
    //    "Notifications",
    //    "Updated"
    //  };
    //  foreach (var oldTicketProperty in oldTicket.GetType().GetProperties())
    //  {
    //    var newTicketProperty = newTicket.GetType().GetProperty(oldTicketProperty.Name);
    //    if (!Object.Equals(oldTicketProperty.GetValue(oldTicket), newTicketProperty.GetValue(newTicket, null)))
    //    {
    //      if (ignoreValues.Contains(oldTicketProperty.Name))
    //      {
    //        continue;
    //      }
    //      TicketHistory history = new TicketHistory
    //      {
    //        TicketId = newTicket.Id,
    //        Property = newTicketProperty.Name,
    //        OldValue = oldTicketProperty.GetValue(oldTicket).ToString(),
    //        NewValue = newTicketProperty.GetValue(newTicket).ToString(),
    //        Created = DateTime.Now,
    //        UserId = userId
    //      };
    //      await _context.TicketHistories.AddAsync(history);
    //    }
    //  }
    //  await _context.SaveChangesAsync();
    //}
    //public async Task AddHistory(Ticket oldTicket, Ticket newTicket, string userId)
    //{
    //  List<PropertyInfo> differences = CheckChanges(oldTicket, newTicket);
    //  foreach (PropertyInfo prop in differences.GetType().GetProperties())
    //  {
    //    var propName = prop.Name;
    //    TicketHistory history = new TicketHistory
    //    {
    //      TicketId = newTicket.Id,
    //      Property = propName,
    //      OldValue = (string)prop.GetValue(oldTicket),
    //      NewValue = (string)prop.GetValue(newTicket),
    //      Created = DateTimeOffset.Now,
    //      UserId = userId
    //    };
    //    await _context.TicketHistories.AddAsync(history);
    //  }
    //  // Save changes
    //}
    //public List<PropertyInfo> CheckChanges(Ticket oldTicket, Ticket newTicket)
    //{
    //  // Filter props

    //  List<PropertyInfo> changes = new List<PropertyInfo>();
    //  foreach(PropertyInfo prop in oldTicket.GetType().GetProperties())
    //  {
    //    object prop1 = prop.GetValue(oldTicket);
    //    object prop2 = prop.GetValue(newTicket);
    //    if (!prop1.Equals(prop2))
    //    {
    //      changes.Add(prop);
    //    }
    //  }
    //  return changes;
    //}

  }
}
