using System;
using System.Reflection;
using Infrastructure.Common.Configs;
using Infrastructure.Common.Configs.AppName;
using Infrastructure.Queue.Attributes;
using Infrastructure.Queue.Configs.Old;
using Infrastructure.Queue.Subscribers;

namespace Infrastructure.Queue.Configs
{
    [Config]
    public class RabbitMqSubscriberConfig
    {
        public RabbitMqSubscriberConfig()
        {
            RetryCount = 0;
        }

        BackCompatibilitySettings CompatibilitySettings { get; set; }

        /// <summary>
        /// Наименование очереди
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// Наименование обменника сообщений
        /// </summary>
        public string ExchangeName { get; set; }

        /// <summary>
        /// Признак включения обменника DLX для сообщений
        /// </summary>
        public bool UseDLX { get; set; }

        /// <summary>
        /// Время жизни сообщения в очереди, в мсек
        /// </summary>
        public int Ttl { get; set; }

        /// <summary>
        /// Количество попыток разбора сообщения
        /// </summary>
        public int RetryCount { get; set; }

        /// <summary>
        /// Общий Dead-Letter-Exchange
        /// </summary>
        public string CommonDlxExchange { get; set; }

        /// <summary>
        /// Лимит количетсва кешируемых сообщений.
        /// </summary>
        public int Limit { get; set; }

        public static RabbitMqSubscriberConfig For<T, TM>()
            where T : ISubscriber<TM>
            where TM : class
        {
            return For<TM>(typeof(T));
        }

        public static RabbitMqSubscriberConfig For<TM>(Type subscriber)
            where TM : class
        {
            var messageConfig = RabbitMqMessageConfig.For<TM>();
            var attr = subscriber.GetCustomAttribute<RabbitMqSubscriberAttribute>(false)
                       ?? new RabbitMqSubscriberAttribute();
            var config = new RabbitMqSubscriberConfig
            {
                ExchangeName = attr.ExchangeName ?? messageConfig.ExchangeName,
                QueueName = attr.QueueName ?? $"{AppName.Name}.{messageConfig.ExchangeName}",
                UseDLX = attr.UseDlx,
                Ttl = attr.Ttl,
                RetryCount = attr.RetryCount,
                Limit = attr.Limit,
            };
            ConfigExtension.FillFromConfig(config, attr.ConfigName ?? config.QueueName, useSubSectionValue: true);
            return config;
        }
    }
}