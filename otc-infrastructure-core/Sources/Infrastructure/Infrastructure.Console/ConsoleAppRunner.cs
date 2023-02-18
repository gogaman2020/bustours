using System;
using Infrastructure.Common.Configs;
using Infrastructure.Common.Logging;
using Infrastructure.Common.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

namespace Infrastructure.Console
{
    public class ConsoleAppRunner
    {
        public static void Start(string[] args)
        {
            LogInfrastructurePlugin.InitDefault();
            var logger = LogManager.GetCurrentClassLogger();

            try
            {
                var host = CreateHostBuilder(args).Build();
                host.Services.Configure();
                host.Run();
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger.Error(ex, $"Завершено с ошибкой: {ex.Message}");

                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseEnvironment(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production")
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.Configure(hostContext.HostingEnvironment.EnvironmentName);
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog()
                .ConfigureServices((hostContext, serviceCollection) =>
                {
                    serviceCollection.AutoRegistration(hostContext.Configuration);
                }).
                UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateOnBuild = false;
                });
    }
}