using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Infrastructure.Common.Plugins;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Web.InfrastructurePlugin
{
    public class InfrastructureWebPlugin : IInfrastructurePlugin
    {
        public static void Configure(IApplicationBuilder appBuilder)
        {
            var plugins = InfrastructurePlugins.Plugins;
            
            appBuilder.ApplicationServices.Configure();

            var webPlugins = plugins
                .OfType<IInfrastructureWebPlugin>()
                .Where(p => !(Disabled?.Contains(p.GetType()) ?? false))
                .ToArray();

            if (webPlugins.Length == 0)
            {
                return;
            }

            var config = appBuilder.ApplicationServices.GetRequiredService<IConfiguration>();

            foreach (var plugin in Ordered(webPlugins))
            {
                plugin.Configure(appBuilder, config);
            }
        }

        private static IEnumerable<IInfrastructureWebPlugin> Ordered(IInfrastructureWebPlugin[] plugins)
        {
            return plugins
                .Select(p => (p.GetType().GetCustomAttribute<WebPluginOrderAttribute>()?.Value ?? int.MaxValue,
                    p.GetType().Name, p))
                .OrderBy(t => t.Item1)
                .ThenBy(t => t.Item2)
                .Select(t => t.Item3);
        }

        public static Type[] Disabled { get; set; } = null;
    }
}