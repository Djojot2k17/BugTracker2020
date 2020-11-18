using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker2020.Models.ViewModels
{
  public class TicketDetailsViewModel
  {
    public Ticket Ticket { get; set; }
    public TicketComment Comment { get; set; }
  }
}
