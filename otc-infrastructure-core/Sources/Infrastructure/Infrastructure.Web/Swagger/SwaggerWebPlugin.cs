using System;
using Infrastructure.Common.Configs.AppName;
using Infrastructure.Web.InfrastructurePlugin;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Infrastructure.Web.Swagger
{
    [WebPluginOrder(10)]
    public class SwaggerWebPlugin : IInfrastructureWebPlugin
    {
        public void RegisterServices(IServiceCollection services, Type[] types, IConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",info: new OpenApiInfo { Title = AppName.Name, Version = "v1" });

                //https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1607#issuecomment-607170559
                c.CustomSchemaIds(type => type.ToString());
            });

        }
        public void Configure(IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", AppName.Name);
            });
        }
    }
}