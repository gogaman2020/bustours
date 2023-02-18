using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Common.Plugins
{
    public interface IInfrastructurePlugin
    {
        void RegisterServices(IServiceCollection services, Type[] types, IConfiguration configuration)
        {
        }

        void Configure(IServiceProvider serviceProvider)
        {
        }
    }
}