using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReversiApp.Controllers;
using Serilog;

namespace ReversiApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
#if (DEBUG)
                    .WriteTo.File($"Logs/Information/InformationLog-.txt",
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                    rollingInterval: RollingInterval.Minute)
#endif
                .WriteTo.File($"Logs/Warnings/WarningLog-.txt",
                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning,
                rollingInterval: RollingInterval.Day)
                .CreateLogger();
            try
            {
                Log.Information("Application is starting up...");
                Log.Warning("This test message can be safely ignored!");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Something went wrong when starting up the application!");
            }
            finally
            {
                Log.Information("Application is closing...");
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
    }
}