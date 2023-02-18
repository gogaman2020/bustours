using Infrastructure.Web.InfrastructurePlugin;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Web.Security
{
    [WebPluginOrder(200)]
    public class AuthWebPlugin : IInfrastructureWebPlugin
    {
        public void Configure(IApplicationBuilder app, IConfiguration configuration)
        {
            if (AuthConfig.UseAuthMiddleware)
            {
                if (AuthConfig.Filter == null)
                {
                    app.UseMiddleware<AuthMiddleware>();
                }
                else
                {
                    app.UseWhen(AuthConfig.Filter,
                        builder =>
                        {
                            builder.UseMiddleware<AuthMiddleware>();
                        });
                }
            }
        }
    }
}