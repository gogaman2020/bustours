using Microsoft.AspNetCore.Builder;

namespace Infrastructure.Web.InfrastructurePlugin
{
    public static class InfrastructureWebPluginExtensions
    {
        public static void AutoConfigure(this IApplicationBuilder app)
        {
            InfrastructureWebPlugin.Configure(app);
        }
    }
}