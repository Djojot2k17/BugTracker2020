using BugTracker2020.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker2020.Services
{
  public interface IBTHistoriesService
  {
    Task AddHistory(Ticket oldTicket, Ticket newTicket, string userId);
  }
}
