using Infrastructure.Web.InfrastructurePlugin;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BusTour.WebApi.Plugins
{
    public class ActionContextAccessorPlugin : IInfrastructureWebPlugin
    {
        public void RegisterServices(IServiceCollection services, Type[] types, IConfiguration configuration)
        {
            services
                .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                .AddHttpContextAccessor();
        }

        public void Configure(IApplicationBuilder app, IConfiguration configuration)
        {
        }
    }
}
