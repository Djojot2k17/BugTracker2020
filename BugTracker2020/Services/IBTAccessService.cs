using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker2020.Services
{
  public interface IBTAccessService
  {
    public Task<bool> CanAccessProject(string userId, int projectId, string roleName);
    public Task<bool> CanAccessTicket(string userId, int ticketId, string roleName);
  }
}
