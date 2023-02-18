using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Common.Plugins
{
    public static class HostConfigExtension
    {
        public static IConfigurationBuilder AddConfigPlugins(this IConfigurationBuilder config, string environmentName)
        {
            var plugins =
                Types.AvailableTypes.Where(t => t.IsClass && t.GetInterface(nameof(IHostConfigPlugin)) != null);
            foreach (var plugin in plugins)
            {
                var instance = (IHostConfigPlugin) Activator.CreateInstance(plugin);
                instance.Add(config, environmentName);
            }
            return config;
        }
    }
}