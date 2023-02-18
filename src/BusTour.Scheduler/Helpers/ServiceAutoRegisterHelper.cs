using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BusTour.Scheduler.Helpers
{
    public static class ServiceAutoRegisterHelper
    {
        public static IServiceCollection AutoRegistration(this IServiceCollection collection)
        {
            foreach (var typeInfo in GetTypes())
            {
                collection.Add(new ServiceDescriptor(typeInfo.impl, typeInfo.impl, typeInfo.lifetime));
                foreach (var iface in typeInfo.ifaces)
                {
                    collection.Add(new ServiceDescriptor(iface, sp => sp.GetService(typeInfo.impl), typeInfo.lifetime));
                }
            }

            return collection;
        }

        private static IEnumerable<(ServiceLifetime lifetime, Type impl, Type[] ifaces)> GetTypes()
        {
            var path = AppContext.BaseDirectory;
            var files = Directory.EnumerateFiles(path, "*.dll");

            foreach (var file in files)
            {
                List<Type> types = null;
                try
                {
                    var assembly = Assembly.LoadFrom(file);
                    types = assembly.GetTypes()
                        .Where(t => t.IsClass && t.GetCustomAttribute<InjectAsAttribute>() != null)
                        .ToList();
                }
                catch (Exception)
                {
                    //skip
                }

                if (types != null)
                {
                    foreach (var type in types)
                    {
                        var attr = type.GetCustomAttribute<InjectAsAttribute>();
                        var ifaces = type.GetInterfaces()
                            .Where(i => attr.Interfaces.Length == 0 || attr.Interfaces.Contains(i))
                            .ToArray();

                        yield return (attr.Lifetime, type, ifaces);
                    }
                }
            }
        }
    }
}
