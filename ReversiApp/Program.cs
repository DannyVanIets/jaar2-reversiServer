using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReversiApp.Controllers;
using Serilog.Core;

namespace ReversiApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Log.Logger = new LoggerConfiguration()
            //    .WriteTo.Console()
            //    .WriteTo.File($"logs/mylog.txt", 
            //    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
            //    rollingInterval: RollingInterval.Minute)
            //    .WriteTo.File($"logs/myerrorlog.txt", 
            //    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning,
            //    rollingInterval: RollingInterval.Minute)
            //    .CreateLogger();
            //try
            //{
            //    Log.Information("Starting the application...");
            //    var host = CreateHostBuilder(args).Build();
            //    Log.Warning("Warning!");
            //    host.Run();
            //}
            //catch(System.Exception ex)
            //{
            //    Log.Fatal(ex, "Exception in application");
            //}
            //finally
            //{
            //    Log.Information("Exciting application");
            //    Log.CloseAndFlush();
            //}

            ILoggerFactory loggerFactory = new LoggerFactory();
            //loggerFactory.AddFile("Logs/Information/InformationLog-{Date}.txt", LogLevel.Information);
            //loggerFactory.AddFile("Logs/Warnings/WarningLog-{Date}.txt", LogLevel.Warning);
            ILogger logger = loggerFactory.CreateLogger<Program>();

            try
            {
                logger.LogInformation("Application is starting up...");
                logger.LogWarning("Something went wrong! This is a test message can be safely ignored.");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                logger.LogCritical(e, "Something went wrong when starting up the application!");
            }
            finally
            {
                logger.LogInformation("Application is closing...");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureLogging(logBuilder =>
        {
            logBuilder.ClearProviders(); // removes all providers from LoggerFactory
            logBuilder.AddConsole();
            logBuilder.AddTraceSource("Information, ActivityTracing"); // Add Trace listener provider
            logBuilder.AddFile("Logs/Warnings/WarningLog-{Date}{Time}.txt", LogLevel.Warning);
            logBuilder.AddFile("Logs/Information/InformationLog-{Date}{Time}.txt", LogLevel.Information);
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
    }
}
