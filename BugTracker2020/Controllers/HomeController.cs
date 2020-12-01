using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BugTracker2020.Models;
using Microsoft.AspNetCore.Authorization;
using BugTracker2020.Data;

namespace BugTracker2020.Controllers
{
  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
      _logger = logger;
      _context = context;
    }
    [Authorize]
    public IActionResult Index()
    {
      TempData["InHomeIndex"] = true;
      //return RedirectToPage("Login", new { area = "Identity", page = "Account" });
      return View();
    }

    public IActionResult Privacy()
    {
      return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public JsonResult GetStatusChartData()
    {
      var result = new List<ChartData>();
      var stati = _context.TicketStatuses.ToList();
      foreach(var status in stati)
      {
        result.Add(new ChartData
        {
          Label = status.Name,
          Data = _context.Tickets.Where(t=> t.TicketStatusId == status.Id).Count()
        });
      }
      return Json(result);
    }

    public JsonResult GetPriorityChartData()
    {
      var result = new List<ChartData>();
      var priorities = _context.TicketPriorities.ToList();
      foreach (var priority in priorities)
      {
        result.Add(new ChartData
        {
          Label = priority.Name,
          Data = _context.Tickets.Where(t => t.TicketPriorityId == priority.Id).Count()
        });
      }
      return Json(result);
    }

    public JsonResult GetTypeChartData()
    {
      // Figure out how to get a list of projects and display tickets associated with that project.
      var result = new List<ChartData>();
      var types = _context.TicketTypes.ToList();
      foreach (var type in types)
      {
        result.Add(new ChartData
        {
          Label = type.Name,
          Data = _context.Tickets.Where(t => t.TicketTypeId == type.Id).Count()
        });
      }
      return Json(result);
    }

    public JsonResult GetTicketCountOnProjects()
    {
      var result = new List<TicketCountData>();
      var projects = _context.Projects.ToList();
      foreach (var project in projects)
      {
        result.Add(new TicketCountData
        {
          ProjectName = project.Name,
          TicketCount = _context.Tickets.Where(t => t.ProjectId == project.Id).Count()
        });
      }
      return Json(result);
    }
  }
}
