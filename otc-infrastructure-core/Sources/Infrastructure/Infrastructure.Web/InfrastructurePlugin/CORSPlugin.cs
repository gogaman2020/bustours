using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Web.InfrastructurePlugin
{
    [WebPluginOrder(1100)]
    public class CORSPlugin : IInfrastructureWebPlugin
    {
        public void RegisterServices(IServiceCollection services, Type[] types, IConfiguration configuration)
        {
            services.AddCors(o =>
                    o.AddPolicy("AllowPolicy",
                        builder =>
                        {
                            builder
                                .SetIsOriginAllowed(_ => true)
                                //.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader()
                                .AllowCredentials();
                        })
                );
        }

        public void Configure(IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseCors("AllowPolicy");
        }
    }
}