using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Infrastructure.Common.DI
{
    /// <summary>
    /// Фабрика сервисов.
    /// </summary>
    /// <typeparam name="T">Тип фабрики.</typeparam>
    /// <param name="key">Ключ.</param>
    /// <returns>Сервис</returns>
    public delegate T TypeFactory<T>(int key);

    /// <summary>
    /// Регистратор типов.
    /// </summary>
    public static class ServiceRegisterHelper
    {
        private static Dictionary<Type, Dictionary<int, Type>> _loadedTypes =
            new Dictionary<Type, Dictionary<int, Type>>();

        /// <summary>
        /// Регистрирует сервисы интерфейса, помеченные атрибутом.
        /// </summary>
        /// <typeparam name="TInterface">Тип интерфейса</typeparam>
        /// <typeparam name="TAttribute">Тип атрибута</typeparam>
        /// <param name="services">DI</param>
        /// <returns>DI</returns>
        public static IServiceCollection RegisterType<TInterface, TAttribute>(this IServiceCollection services)
            where TAttribute : Attribute, IKeyIdentity
        {
            var types = Types.AvailableTypes;
            var itype = typeof(TInterface);
            var foundedTypes = types.Where(p => itype.IsAssignableFrom(p) && !p.IsInterface)
                .ToList();

            Dictionary<int, Type> concreteTypes;
            if (!_loadedTypes.TryGetValue(itype, out concreteTypes))
            {
                concreteTypes = new Dictionary<int, Type>();
                _loadedTypes[itype] = concreteTypes;
            }

            foreach (var type in foundedTypes)
            {
                var attributes = type.GetCustomAttributes(typeof(TAttribute), true)
                    .OfType<TAttribute>()
                    .ToArray();

                foreach (var attribute in attributes)
                {
                    if (!concreteTypes.ContainsKey(attribute.Key))
                    {
                        concreteTypes.Add(attribute.Key, type);
                        services.TryAddScoped(type);
                    }
                    else
                    {
                        throw new ArgumentException($"Сервис {itype.Name} - {attribute.Key} уже зарегистрирован");
                    }
                }
            }

            services.AddScoped<TypeFactory<TInterface>>(factory => key => GetService<TInterface>(factory, key));

            return services;
        }

        /// <summary>
        /// Резолвит сервис по ключу
        /// </summary>
        /// <typeparam name="T">Тип сервиса</typeparam>
        /// <param name="serviceProvider">DI</param>
        /// <param name="key">Ключ сервиса</param>
        /// <returns>Сервис</returns>
        public static T GetService<T>(this IServiceProvider serviceProvider, int key)
        {
            if (_loadedTypes.TryGetValue(typeof(T), out var concreteTypes))
            {
                if (concreteTypes.TryGetValue(key, out var type))
                {
                    return (T) serviceProvider.GetService(type);
                }

                throw new Exception($"Не удалось найти сервис {typeof(T).Name} - {key}");
            }

            throw new Exception($"Не удалось найти зарегистрированный тип - {typeof(T).Name}");
        }
    }
}