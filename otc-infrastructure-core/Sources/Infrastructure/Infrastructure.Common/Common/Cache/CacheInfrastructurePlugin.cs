using System;
using Infrastructure.Common.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Common.Cache
{
    public class CacheInfrastructurePlugin : IInfrastructurePlugin
    {
        public void RegisterServices(IServiceCollection services, Type[] types, IConfiguration configuration)
        {
            services.AddMemoryCache();
        }
    }
}