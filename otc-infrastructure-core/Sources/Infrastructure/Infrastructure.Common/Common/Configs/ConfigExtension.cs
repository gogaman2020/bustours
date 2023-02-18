using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Infrastructure.Common.DI;
using Infrastructure.Common.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Common.Configs
{
    public static class ConfigExtension
    {
        private static MethodInfo _method = typeof(ConfigExtension)
            .GetMethod(nameof(Configure), BindingFlags.Static | BindingFlags.NonPublic);

        private static ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>> _fillMappings =
            new ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>>();

        public static IServiceCollection Configure(this IServiceCollection services, Type type, string name,
            IConfiguration configuration = null)
        {
            configuration ??= IoC.GetService<IConfiguration>();
            var tmethod = _method.MakeGenericMethod(type);
            tmethod.Invoke(null, new object[] {services, configuration.GetSection(name)});
            return services;
        }

        public static void FillFromConfig<T>(T obj, string name, IConfiguration configuration = null, bool skipHiddenFields = false, bool useSubSectionValue = false)
            where T : class, new()
        {
            HashSet<string> hidden = null;
            if (!skipHiddenFields)
            {
                var hf = typeof(T).GetCustomAttribute<ConfigAttribute>(false)?.HiddenFields;
                if ((hf?.Length ?? 0) > 0)
                {
                    hidden = new HashSet<string>(hf);
                }
            }
            
            var maps = _fillMappings.GetOrAdd(obj.GetType(), t => t.GetProperties()
                .Where(p => p.CanWrite)
                .ToDictionary(p => p.Name));
            
            configuration ??= IoC.GetService<IConfiguration>();
            
            //берем обьект конфигурации распарсенный системой
            var configValues = Config.Get<T>();
            
            //секция в конфиге
            var section = configuration.GetSection(name);

            //для свойств оперделенных в конфиге копируем значение из обьекта конфига     
            foreach (var child in section.GetChildren())
            {
                if (hidden?.Contains(child.Key) ?? false)
                {
                    continue;
                }

                if (maps.TryGetValue(child.Key, out var propertyInfo))
                {
                    if (useSubSectionValue)
                    {
                        if (string.IsNullOrEmpty(child.Value))
                        {
                            propertyInfo.SetValue(obj, null);
                        }
                        else
                        {
                            propertyInfo.SetValue(obj, Convert.ChangeType(child.Value, propertyInfo.PropertyType));
                        }
                    }
                    else
                    {
                        propertyInfo.SetValue(obj, propertyInfo.GetValue(configValues));
                    }
                }
            }
        }

        private static void Configure<T>(IServiceCollection services, IConfiguration section)
            where T : class
        {
            services.Configure<T>(section);
        }

        public static string GetConfigName(Type type)
        {
            var attr = type.GetCustomAttribute<ConfigAttribute>(false);
            var hasName = !string.IsNullOrEmpty(attr?.DefaultConfigName);
            var name = ConfigMapConfig.GetName(type, !hasName) ?? attr?.DefaultConfigName;
            return name;
        }

        public static IConfigurationBuilder Configure(this IConfigurationBuilder config, string environmentName)
        {
            const string ConfigPath = "Configs";
            config.SetBasePath(Path.Combine(AppContext.BaseDirectory, ConfigPath));
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            if (!string.IsNullOrEmpty(environmentName))
            {
                config.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
            }

            config.AddConfigPlugins(environmentName);
            config.AddEnvironmentVariables();
            return config;
        }
    }
}