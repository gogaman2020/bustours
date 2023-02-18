using System;
using Infrastructure.Common.Plugins;
using Infrastructure.Db.Audit;
using Infrastructure.Db.ConnectionFactories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Infrastructure.Db.InfrastructurePlugin
{
    public class DbInfrastructurePlugin : IInfrastructurePlugin
    {
        public void RegisterServices(IServiceCollection services, Type[] types, IConfiguration configuration)
        {
            services
                .AddSingleton(ConnectionFactoryDiFactory.GetFactory)
                .AddSingleton(ConnectionFactoryDiFactory.GetFormatter)
                ;
        }
    }
}