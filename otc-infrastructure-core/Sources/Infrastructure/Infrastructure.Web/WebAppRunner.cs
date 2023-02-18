using System;
using System.IO;
using System.Reflection;
using Infrastructure.Common.Configs;
using Infrastructure.Common.Logging;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Web;

namespace Infrastructure.Web
{
    public class WebAppBase
    {
        public static Assembly StartAssembly { get; protected set; }
        public static Action<IServiceCollection> Register { get; protected set; }
        public static Action<IApplicationBuilder> Configure { get; protected set; }
        public static Action<IApplicationBuilder, IWebHostEnvironment> ConfigureWithEnv { get; protected set; }
    }

    public class WebAppRunner<TStartup> : WebAppBase
        where TStartup : class
    {
        public static void Start(string[] args, Action<IServiceCollection> register = null,
            Action<IApplicationBuilder> configure = null, Action<IApplicationBuilder, IWebHostEnvironment> configureWithEnv = null)
        {
            StartAssembly = Assembly.GetCallingAssembly();
            Logger logger = null;
            Register = register;
            Configure = configure;
            ConfigureWithEnv = configureWithEnv;

            try
            {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                LogInfrastructurePlugin.InitDefault();
                
                var configPath = Path.Combine("Configs", "NLog.config");
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (!string.IsNullOrEmpty(environment))
                {
                    var path = Path.Combine("Configs", $"NLog.{environment}.config");
                    if (File.Exists(configPath))
                    {
                        configPath = path;
                        NLogBuilder.ConfigureNLog(configPath);
                    }
                }
                else if (File.Exists(configPath))
                {
                    NLogBuilder.ConfigureNLog(configPath);
                }

                logger = LogManager.GetCurrentClassLogger();

                CreateWebHostBuilder(args)
                    .Build()
                    .Run();
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

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                //.UseApplicationInsights()
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.Configure(hostContext.HostingEnvironment.EnvironmentName);
                })
                .ConfigureLogging(logging =>
                {
                    logging.AutoConfigure();
                })
                .UseNLog()
                .UseStartup<TStartup>();
    }
}