using System;
using Infrastructure.Common.DI;
using Infrastructure.Common.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Common.Plugins
{
    public static class InfrastructurePluginExtensions
    {
        public static IServiceCollection AutoRegistration(this IServiceCollection collection,
            IConfiguration configuration)
        {
            foreach (var startupRegistration in InfrastructurePlugins.Plugins)
            {
                startupRegistration.RegisterServices(collection, Types.AvailableTypes, configuration);
            }

            return collection;
        }

        public static void Configure(this IServiceProvider serviceProvider)
        {
            IoC.Init(serviceProvider);

            foreach (var startupRegistration in InfrastructurePlugins.Plugins)
            {
                startupRegistration.Configure(serviceProvider);
            }

            InfrastructurePlugins.Cleanup();
            Types.Cleanup();
        }
    }
}