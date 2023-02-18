using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Infrastructure.Common.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Common.DI
{
    public class DiInfrastructurePlugin : IInfrastructurePlugin
    {
        public void RegisterServices(IServiceCollection services, Type[] types, IConfiguration configuration)
        {
            foreach (var typeInfo in GetInjectTypes())
            {
                services.Add(new ServiceDescriptor(typeInfo.impl, typeInfo.impl, typeInfo.lifetime));
                if (typeInfo.impl.IsGenericTypeDefinition)
                {
                    foreach (var iface in typeInfo.ifaces)
                    {
                        var tiface = iface;
                        //если в интерфейсе генерике определены типы-параметры,
                        //то берем базовый генерик без определенных типов
                        if (iface.IsGenericType
                            && !iface.IsGenericTypeDefinition
                            && iface.GenericTypeArguments.Any(t=>t.IsGenericTypeParameter))
                        {
                            tiface = iface.GetGenericTypeDefinition();
                        }
                        services.Add(
                            new ServiceDescriptor(tiface, typeInfo.impl, typeInfo.lifetime));
                    }
                }
                else
                {
                    foreach (var iface in typeInfo.ifaces)
                    {
                        services.Add(
                            new ServiceDescriptor(iface, sp => sp.GetService(typeInfo.impl), typeInfo.lifetime));
                    }
                }
            }
        }
        
        private static IEnumerable<(ServiceLifetime lifetime, Type impl, Type[] ifaces)> GetInjectTypes()
        {
            return Types.AvailableTypes
                .Where(t => t.IsClass && t.GetCustomAttribute<InjectAsAttribute>(false) != null)
                .Select(type =>
                {
                    if (type.IsAbstract)
                    {
                        throw new ArgumentException($"Type {type.FullName} is abstract with InjectAs attribute");
                    }

                    var attr = type.GetCustomAttribute<InjectAsAttribute>(false);
                    var ifaces = type.GetInterfaces()
                        .Where(i => attr.Interfaces.Length == 0 || attr.Interfaces.Contains(i))
                        .ToArray();



                    return (attr.Lifetime, type, ifaces);
                });
        }
    }
}