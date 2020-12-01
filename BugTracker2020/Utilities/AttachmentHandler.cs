using BugTracker2020.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker2020.Utilities
{
  public class AttachmentHandler
  {
    // Maybe there's a better way to do this....
    public TicketAttachment AttachToTicket(IFormFile attachment, int Id)
    {
      // Get data and filepath
      var memoryStream = new MemoryStream();
      attachment.CopyTo(memoryStream);
      byte[] bytes = memoryStream.ToArray();
      memoryStream.Close();
      memoryStream.Dispose();
      var binary = Convert.ToBase64String(bytes);
      var ext = Path.GetExtension(attachment.FileName);
      // Stick it on the ticketAttachment
      var ticketAttachment = new TicketAttachment();
      ticketAttachment.TicketId = Id;
      ticketAttachment.FilePath = $"data:image/{ext};base64,{binary}";
      ticketAttachment.FileData = bytes;
      ticketAttachment.Description = Path.GetFileNameWithoutExtension(attachment.FileName.Replace(" ", "_"));
      ticketAttachment.Created = DateTime.Now;
      return ticketAttachment;
    }
    public Project AttachToProject(IFormFile attachment)
    {
      var project = new Project();
      if (attachment != null)
      {
        // Get data and filepath
        var memoryStream = new MemoryStream();
        attachment.CopyTo(memoryStream);
        byte[] bytes = memoryStream.ToArray();
        memoryStream.Close();
        memoryStream.Dispose();
        var binary = Convert.ToBase64String(bytes);
        var ext = Path.GetExtension(attachment.FileName);
        
        // Stick it in the project
        project.ImagePath = $"data:image/{ext};base64,{binary}";
        project.ImageData = bytes;
      }
      return project;
    }
  }
}
