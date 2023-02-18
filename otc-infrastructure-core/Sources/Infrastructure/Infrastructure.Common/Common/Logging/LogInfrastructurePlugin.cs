using System;
using System.IO;
using System.Text;
using System.Xml;
using Infrastructure.Common.Configs;
using Infrastructure.Common.Configs.AppName;
using Infrastructure.Common.DI;
using Infrastructure.Common.Helpers;
using Infrastructure.Common.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Infrastructure.Common.Logging
{
    public class LogInfrastructurePlugin : IInfrastructurePlugin
    {
        private class Logger<T> : ILogger<T>
        {
            private readonly ILogger _logger = ApplicationLogging.CreateLogger<T>();

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
                Func<TState, Exception, string> formatter)
            {
                _logger.Log<TState>(logLevel, eventId, state, exception, formatter);
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return _logger.IsEnabled(logLevel);
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return _logger.BeginScope<TState>(state);
            }
        }

        public void RegisterServices(IServiceCollection services, Type[] types, IConfiguration configuration)
        {
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
        }

        public void Configure(IServiceProvider serviceProvider)
        {
            var appname = Config.Get<AppNameConfig>().Name;
            var logPath = Config.Get<LogConfig>()?.Path;
            var config = GetConfigText();
            if (string.IsNullOrEmpty(logPath))
            {
                config = config.Replace("Startup", appname);
            }
            else
            {
                logPath = logPath.Replace("{{APPNAME}}", appname);
                config = config.Replace(
                    "${environment:variable=ALLUSERSPROFILE}/Rts/BaseTrade/Logs/Startup/${date:format=yyyy}/${date:format=MM}/${date:format=dd}/",
                    logPath);
            }

            var xmlFileReader = XmlReader.Create(new StringReader(config));
            LogManager.Configuration = new XmlLoggingConfiguration(xmlFileReader);
            ApplicationLogging.LoggerFactory = IoC.GetRequiredService<ILoggerFactory>();
        }

        public static string GetConfigText()
        {
            var streamName = $"{typeof(LogInfrastructurePlugin).Namespace}.Config.NLog.config";
            using (var stream = typeof(LogInfrastructurePlugin).Assembly.GetManifestResourceStream(streamName))
            {
                if (stream == null)
                {
                    throw new Exception($"Файл {streamName} не найден");
                }

                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static void InitDefault()
        {
            var configText = GetConfigText();
            var xmlFileReader = XmlReader.Create(new StringReader(configText));
            LogManager.Configuration = new XmlLoggingConfiguration(xmlFileReader);
            ApplicationLogging.LoggerFactory = new NLogLoggerFactory();
        }
    }
}