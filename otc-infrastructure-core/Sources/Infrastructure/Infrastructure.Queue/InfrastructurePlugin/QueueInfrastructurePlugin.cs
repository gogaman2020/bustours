using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Infrastructure.Common.Configs;
using Infrastructure.Common.Plugins;
using Infrastructure.Queue.Attributes;
using Infrastructure.Queue.Configs;
using Infrastructure.Queue.Publishers;
using Infrastructure.Queue.Subscribers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Queue.InfrastructurePlugin
{
    public class QueueInfrastructurePlugin : IInfrastructurePlugin
    {
        public static IReadOnlyList<Type> Subscribers { get; private set; }

        private readonly MethodInfo _methodSubscribe =
            typeof(QueueInfrastructurePlugin).GetMethod("Subscribe", BindingFlags.Static | BindingFlags.NonPublic);

        public void RegisterServices(IServiceCollection services, Type[] types, IConfiguration configuration)
        {
            services.AddSingleton(typeof(IPublisher<>), typeof(Publisher<>));

            Subscribers = types
                .Where(t => t.GetInterfaces()
                    .Any(i => i.IsConstructedGenericType
                              && i.GetGenericTypeDefinition() == typeof(ISubscriber<>)))
                .ToArray();

            if (Subscribers.Count == 0)
            {
                return;
            }

            CheckAttributes();

            foreach (var subscriber in Subscribers)
            {
                var attr = subscriber.GetCustomAttribute<RabbitMqSubscriberAttribute>()
                           ?? new RabbitMqSubscriberAttribute();
                if (!string.IsNullOrEmpty(attr.ConfigName))
                {
                    services.Configure(subscriber, attr.ConfigName, configuration);
                }
            }
        }

        public void Configure(IServiceProvider serviceProvider)
        {
            var manager = serviceProvider.GetRequiredService<SubscribersManager>();
            foreach (var subscriberType in Subscribers)
            {
                var ifaceType = subscriberType
                    .GetInterfaces()
                    .FirstOrDefault(i => i.IsConstructedGenericType
                                         && i.GetGenericTypeDefinition() == typeof(ISubscriber<>));
                var mattr = ifaceType?.GenericTypeArguments[0].GetCustomAttribute<RabbitMqMessageAttribute>()
                            ?? new RabbitMqMessageAttribute(oldMessage: true);

                if (mattr.OldMessage
                    && subscriberType.GetInterfaces().Any(i => i == typeof(IHostedService)))
                {
                    continue;
                }

                var method = _methodSubscribe.MakeGenericMethod(subscriberType, ifaceType.GenericTypeArguments[0]);
                manager.Add(() => (IDisposable) method.Invoke(null, new object[] {serviceProvider}));
            }
        }

        /// <summary>
        /// Это _methodSubscribe
        /// </summary>
        private static IDisposable Subscribe<T, TM>(IServiceProvider serviceProvider)
            where T : ISubscriber<TM>
            where TM : class
        {
            var subscriber = (ISubscriber<TM>) serviceProvider.GetRequiredService<T>();
            var manager = serviceProvider.GetRequiredService<ISubscriberFactory>();
            var disposable = manager.Subscribe(subscriber);
            return disposable;
        }

        private static void CheckAttributes()
        {
            var errors = Subscribers
                .SelectMany(CheckAttributes)
                .Where(s => !string.IsNullOrEmpty(s));

            var result = string.Join(Environment.NewLine, errors);
            if (!string.IsNullOrEmpty(result))
            {
                throw new ArgumentException(result);
            }
        }

        private static IEnumerable<string> CheckAttributes(Type subscriber)
        {
            var attr = subscriber.GetCustomAttribute<RabbitMqSubscriberAttribute>();
            if (attr == null)
            {
                yield return $"Тип {subscriber.FullName} не содержит атрибута {nameof(RabbitMqSubscriberAttribute)}";
            }

            var mtype = subscriber
                .GetInterfaces()
                .FirstOrDefault(i => i.IsConstructedGenericType
                                     && i.GetGenericTypeDefinition() == typeof(ISubscriber<>))
                ?.GenericTypeArguments[0];
            var mattr = mtype?.GetCustomAttribute<RabbitMqMessageAttribute>();
            if (mattr == null)
            {
                yield return $"Тип сообщения {mtype?.FullName} не содержит атрибута {nameof(RabbitMqMessageAttribute)}";
            }
        }
    }
}