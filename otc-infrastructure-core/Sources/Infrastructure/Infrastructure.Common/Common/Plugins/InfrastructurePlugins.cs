using System;
using System.Linq;

namespace Infrastructure.Common.Plugins
{
    public static class InfrastructurePlugins
    {
        public static IInfrastructurePlugin[] Plugins =>
            _infrastructurePlugins?.Value ?? Array.Empty<IInfrastructurePlugin>();

        public static void Cleanup()
        {
            _infrastructurePlugins = null;
        }

        private static Lazy<IInfrastructurePlugin[]> _infrastructurePlugins =
            new Lazy<IInfrastructurePlugin[]>(GetStartupRegistrations);

        private static IInfrastructurePlugin[] GetStartupRegistrations()
        {
            return Types.AvailableTypes
                .Where(t => !(Disabled?.Contains(t) ?? false))
                .Where(t => t.IsClass && t.GetInterface(nameof(IInfrastructurePlugin)) != null)
                .Select(t => (IInfrastructurePlugin) Activator.CreateInstance(t))
                .ToArray();
        }

        public static Type[] Disabled { get; set; } = null;
    }
}