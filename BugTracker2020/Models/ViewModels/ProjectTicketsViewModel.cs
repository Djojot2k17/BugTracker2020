using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker2020.Models.ViewModels
{
  public class ProjectTicketsViewModel
  {
    public List<Ticket> Tickets { get; set; }
    public Project Project { get; set; }
  }
}
