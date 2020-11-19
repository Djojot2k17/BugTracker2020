using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker2020.Data;
using BugTracker2020.Models;
using BugTracker2020.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BugTracker2020
{
  public class Program
  {
    public async static Task Main(string[] args)
    {
      //CreateHostBuilder(args).Build().Run();
      var host = CreateHostBuilder(args).Build();
      await DataHelper.ManageData(host);
      //using (var scope = host.services.createscope())
      //{
      //  var services = scope.serviceprovider;
      //  var loggerfactory = services.getrequiredservice<iloggerfactory>();
      //  try
      //  {
      //    var context = services.getrequiredservice<applicationdbcontext>();
      //    var usermanager = services.getrequiredservice<usermanager<btuser>>();
      //    var rolemanager = services.getrequiredservice<rolemanager<identityrole>>();
      //    await contextseed.seedrolesasync(rolemanager);
      //    await contextseed.seeddefaultusersasync(usermanager);
      //    await contextseed.seeddefaultticketpropsasync(context);
      //  }
      //  catch (exception ex)
      //  {
      //    var logger = loggerfactory.createlogger<program>();
      //    logger.logerror(ex, "an error ocurred seeding the database.");
      //  }
      //}
      host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
              webBuilder.CaptureStartupErrors(true);
              webBuilder.UseSetting(WebHostDefaults.DetailedErrorsKey, "true");
              webBuilder.UseStartup<Startup>();
            });
  }
}
