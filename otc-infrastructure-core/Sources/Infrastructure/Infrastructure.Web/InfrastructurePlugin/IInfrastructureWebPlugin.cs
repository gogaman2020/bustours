using Infrastructure.Common.Plugins;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Web.InfrastructurePlugin
{
    public interface IInfrastructureWebPlugin : IInfrastructurePlugin
    {
        void Configure(IApplicationBuilder app, IConfiguration configuration);
    }
}