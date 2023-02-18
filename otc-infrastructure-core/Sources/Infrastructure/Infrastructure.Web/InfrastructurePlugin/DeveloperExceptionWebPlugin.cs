using Infrastructure.Web.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Web.InfrastructurePlugin
{
    [WebPluginOrder(1)]
    public class DeveloperExceptionWebPlugin : IInfrastructureWebPlugin
    {
        public void Configure(IApplicationBuilder app, IConfiguration configuration)
        {
            var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.ConfigureExceptionHandler();
            }
        }
    }
}