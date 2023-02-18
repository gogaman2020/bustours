using System.Reflection;
using Infrastructure.Common.Configs;
using Infrastructure.Queue.Attributes;

namespace Infrastructure.Queue.Configs
{
    [Config]
    public class RabbitMqMessageConfig
    {
        /// <summary>
        /// Наименование обменника сообщений
        /// </summary>
        public string ExchangeName { get; set; }
        public bool OldMessage { get; set; }
        public bool PersistentMessage { get; set; }
        public string ExchangeType { get; set;  }
        public string RoutingKey { get; set;  }

        public static RabbitMqMessageConfig For<T>()
            where T : class
        {
            var attr = typeof(T).GetCustomAttribute<RabbitMqMessageAttribute>(false) 
                       ?? new RabbitMqMessageAttribute();
            
            var config = new RabbitMqMessageConfig
            {
                ExchangeName = attr.ExchangeName ?? typeof(T).Name,
                OldMessage = attr.OldMessage,
                PersistentMessage = attr.PersistentMessage,
                ExchangeType = attr.ExchangeType,
                RoutingKey  = attr.RoutingKey
            };
            ConfigExtension.FillFromConfig(config, attr.ConfigName ?? config.ExchangeName);
            
            return config;
        }
    }
}
