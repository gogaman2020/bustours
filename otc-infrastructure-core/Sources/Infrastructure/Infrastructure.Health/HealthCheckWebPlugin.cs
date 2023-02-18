using System;
using Infrastructure.Health.Config;
using Infrastructure.Web.InfrastructurePlugin;
using Infrastructure.Web.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Health
{
    [WebPluginOrder(110)]
    public class HealthCheckWebPlugin : IInfrastructureWebPlugin
    {
        public void RegisterServices(IServiceCollection services, Type[] types, IConfiguration configuration)
        {
            var builder = services.UseHealthCheck();

            if (!HealthCheckConfig.Db.Disabled)
            {
                foreach (var config in HealthCheckConfig.Db.ConfigNames)
                {
                    builder.UseDbCheck(configuration[$"{config}:ConnectionString"]);
                }
            }
            if (!HealthCheckConfig.RabbitMq.Disabled)
            {
                foreach (var config in HealthCheckConfig.RabbitMq.ConfigNames)
                {
                    builder.UseRabbitCheck(configuration["{config}:Host"],
                        configuration["{config}:VirtualHost"],
                        configuration["{config}:User"],
                        configuration["{config}:Password"]);
                }
            }

            if (!HealthCheckConfig.Clients.Disabled)
            {
                builder.UseClientsCheck(services);
            }
        }

        public void Cleanup()
        {
        }

        public void Configure(IApplicationBuilder app, IConfiguration configuration)
        {
            var path = configuration["HealthConfig:Route"];
            app.UseHealthCheck(path);
            if (AuthConfig.Filter == null)
            {
                AuthConfig.Filter = c => c.Request.Path.Value != path;
            }
            else
            {
                var func = AuthConfig.Filter; 
                AuthConfig.Filter = c => func(c) && c.Request.Path.Value != path;
            }
        }
    }
}