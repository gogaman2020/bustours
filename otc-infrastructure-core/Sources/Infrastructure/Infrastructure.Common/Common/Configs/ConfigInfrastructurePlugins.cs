using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Infrastructure.Common.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Common.Configs
{
    public class ConfigInfrastructurePlugins : IInfrastructurePlugin
    {
        public static IReadOnlyList<Type> ConfigTypes { get; private set; }

        public void RegisterServices(IServiceCollection services, Type[] types, IConfiguration configuration)
        {
            services.AddOptions();
            ConfigTypes = types
                .Where(t => t.IsClass &&
                            t.GetCustomAttribute<ConfigAttribute>(false) != null)
                .ToArray();

            foreach (var type in ConfigTypes)
            {
                var name = ConfigExtension.GetConfigName(type);
                services.Configure(type, name, configuration);
            }
        }
    }
}