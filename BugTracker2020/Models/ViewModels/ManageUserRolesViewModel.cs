using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker2020.Models.ViewModels
{
  public class ManageUserRolesViewModel
  {
    public BTUser User { get; set; }
    public SelectList Roles { get; set; }
    public IEnumerable<string> UserRole {get; set;}
    public string SelectedRole { get; set; }
  }
}
