using BusTour.Scheduler.Clients;
using BusTour.Scheduler.Factories;
using BusTour.Scheduler.Helpers;
using BusTour.Scheduler.Options;
using BusTour.Scheduler.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.IO;

namespace BusTour.Scheduler
{
    public class Program
    {
        private static Logger logger;

        public static void Main(string[] args)
        {
            var aspNetCoreEnviromentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            logger = NLogBuilder.ConfigureNLog(Path.Combine("Configs", $"nlog.{aspNetCoreEnviromentName}.config")).GetCurrentClassLogger();

            try
            {
                logger.Debug("Init main");
                logger.Debug("Main - " + aspNetCoreEnviromentName);
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseEnvironment(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    logger.Debug("CreateHostBuilder - " + hostContext.HostingEnvironment.EnvironmentName);
                    config.SetBasePath(AppContext.BaseDirectory);
                    config.AddJsonFile(Path.Combine("Configs", "appsettings.json"), optional: false, reloadOnChange: true);
                    config.AddJsonFile(Path.Combine("Configs", $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json"), optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    //logging.AddConsole();
                })
                .UseNLog()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AutoRegistration();

                    services
                        .Configure<SchedulerConfig>(hostContext.Configuration.GetSection("SchedulerConfig"))
                        .Configure<ClientsConfig>(hostContext.Configuration.GetSection("ClientsConfig"));

                    services
                        .AddSingleton<IJobFactory, SingletonJobFactory>()
                        .AddSingleton<ISchedulerFactory, StdSchedulerFactory>()
                        .AddSingleton<IBusTourClient, BusTourClient>()
                        .AddHostedService<QuartzHostedService>();

                    var serviceProvider = services.BuildServiceProvider();

                    var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                    ApplicationLogging.LoggerFactory = loggerFactory;
                });
    }
}
