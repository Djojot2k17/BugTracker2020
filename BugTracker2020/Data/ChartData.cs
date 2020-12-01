using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker2020.Data
{
  public class ChartData
  {
    public string Label { get; set; }
    public int Data { get; set; }
  }

  public class TicketCountData
  {
    public string ProjectName { get; set; }
    public int TicketCount { get; set; }
  }
}
