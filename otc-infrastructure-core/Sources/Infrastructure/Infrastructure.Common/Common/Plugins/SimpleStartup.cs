using System;
using System.Collections.Generic;
using Infrastructure.Common.Configs;
using Infrastructure.Common.DI;
using Infrastructure.Common.Helpers;
using Infrastructure.Common.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace Infrastructure.Common.Plugins
{
    public static class SimpleStartup
    {
        private class ServiceCollection : List<ServiceDescriptor>, IServiceCollection
        {
        }

        private class LoggingBuilder : ILoggingBuilder
        {
            public LoggingBuilder(IServiceCollection services)
            {
                Services = services;
            }

            public IServiceCollection Services { get; }
        }

        public static IServiceProvider Build(string environmentName = null,
            Action<IServiceCollection> forReplaceAutoRegistered = null)
        {
            LogInfrastructurePlugin.InitDefault();

            var builder = new ConfigurationBuilder();
            var config = builder
                .Configure(environmentName)
                .Build();

            IServiceCollection services = new ServiceCollection();
            services.AutoRegistration(config);
            services.AddSingleton<ILoggerFactory, NLogLoggerFactory>();

            var loggingBuilder = new LoggingBuilder(services);
            loggingBuilder.AutoConfigure();

            forReplaceAutoRegistered?.Invoke(services);
            var sp = services.BuildServiceProvider();
            sp.Configure();
            return sp;
        }
    }
}