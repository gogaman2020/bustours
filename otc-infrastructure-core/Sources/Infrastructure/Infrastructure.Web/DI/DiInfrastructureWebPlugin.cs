using System;
using Infrastructure.Web.InfrastructurePlugin;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Web.DI
{
    [WebPluginOrder(100)]
    public class DiInfrastructureWebPlugin : IInfrastructureWebPlugin
    {
        public void RegisterServices(IServiceCollection services, Type[] types, IConfiguration configuration)
        {
        }

        public void Configure(IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseIoCMiddleware();
        }
    }
}