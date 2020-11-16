using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker2020.Models.ViewModels
{
  public class ManageProjectUsersViewModel
  {
    public BTUser User { get; set; }
    public IEnumerable<string> UserRole { get; set; }
    public ICollection<Project> CurrentProjects { get; set; }
    public SelectList Projects { get; set; }
    public int SelectedProject { get; set; }
  }
}
