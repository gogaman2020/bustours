using System;
using System.Linq;
using System.Reflection;
using Dapper;
using Infrastructure.Common.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Db.TypeHandlers
{
    public class DapperTypeHandlerInfrastructurePlugin : IInfrastructurePlugin
    {
        private Type[] _handlers;

        public void RegisterServices(IServiceCollection services, Type[] types, IConfiguration configuration)
        {
            _handlers = types
                .Where(t => t.GetCustomAttribute<DapperTypeHandlerAttribute>(true) != null
                            && t.GetCustomAttribute<DapperTypeHandlerAttribute>(false) == null)
                .ToArray();
            foreach (var handler in _handlers)
            {
                services.AddSingleton(handler);
            }
        }

        public void Configure(IServiceProvider serviceProvider)
        {
            var method = GetRegistrationMethod();
            foreach (var handler in _handlers)
            {
                var concreteMethod = method.MakeGenericMethod(GetParamType(handler));
                concreteMethod.Invoke(null, new[] {serviceProvider.GetRequiredService(handler)});
            }
        }

        private Type GetParamType(Type classType)
        {
            if (classType == null)
            {
                return null;
            }
            
            if (classType.Name.StartsWith(typeof(DapperTypeHandler<>).Name))
            {
                return classType.GetGenericArguments()[0];
            }

            return GetParamType(classType.BaseType);
        }

        private static void AddHandler<T>(SqlMapper.TypeHandler<T> handler)
        {
            SqlMapper.AddTypeHandler(handler);
        }

        private MethodInfo GetRegistrationMethod()
        {
            var method = GetType().GetMethod(nameof(AddHandler), BindingFlags.Static | BindingFlags.NonPublic);
            return method;
        }
    }
}