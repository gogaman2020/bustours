using ClientsCommon;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Health
{
    public static class HealthExtentions
    {
        public static IHealthChecksBuilder UseHealthCheck(this IServiceCollection services)
        {
            return services.AddHealthChecks();
        }

        public static IHealthChecksBuilder UseDbCheck(this IHealthChecksBuilder services, string connectionString)
        {
#if USE_POSTGRES
            return services.AddNpgSql(connectionString, tags: HealthTags.DB);
#else
            return services.AddSqlServer(connectionString, tags: HealthTags.DB);
#endif
        }

        public static IHealthChecksBuilder UseClientsCheck(this IHealthChecksBuilder builder, IServiceCollection services)
        {
            var checkType = typeof(ICommonClient);
            var generic = typeof(ClientCheck<>);
            var registered = new List<string>();
            foreach (var service in services.ToArray())
            {
                if (checkType.IsAssignableFrom(service.ServiceType))
                {
                    if (!registered.Contains(service.ServiceType.Name))
                    {
                        var constructed = generic.MakeGenericType(new[] { service.ServiceType });
                        var instance = (IHealthCheck)Activator.CreateInstance(constructed);
                        builder.AddCheck(service.ServiceType.Name, instance, HealthStatus.Unhealthy, HealthTags.CLIENT);
                        registered.Add(service.ServiceType.Name);
                    }
                }
            }

            return builder;
        }

        public static IHealthChecksBuilder UseClientCheck<T>(this IHealthChecksBuilder services, string name)
            where T : ICommonClient
        {
            return services.AddCheck<ClientCheck<T>>(name, tags: HealthTags.CLIENT);
        }

        public static IHealthChecksBuilder UseRabbitCheck(this IHealthChecksBuilder services, string host, string virtualHost, string user, string password)
        {
            var rabbitMqCheck = new RabbitMQHealthCheck(host, virtualHost, user, password);
            return services.AddCheck(HealthTags.RABBITMQ.First(), rabbitMqCheck, HealthStatus.Unhealthy, HealthTags.RABBITMQ);
        }

        public static IApplicationBuilder UseHealthCheck(this IApplicationBuilder app, string route)
        {
            app.UseHealthChecks(
                route,
                new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

            return app;
        }
        
    }
}
