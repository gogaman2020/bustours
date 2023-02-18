using System;

namespace Infrastructure.Queue.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RabbitMqMessageAttribute : Attribute
    {
        public RabbitMqMessageAttribute(bool persistentMessage = true, bool oldMessage = false, string configName = null,
            string exchangeName = null, string exchangeType = RabbitMQ.Client.ExchangeType.Fanout, string routingKey = null)
        {
            OldMessage = oldMessage;
            PersistentMessage = persistentMessage;
            ConfigName = configName;
            ExchangeName = exchangeName;
            ExchangeType = exchangeType;
            RoutingKey = routingKey;

        }

        public bool OldMessage { get; }
        public bool PersistentMessage { get; }
        public string ConfigName { get; }
        public string ExchangeName { get; }
        public string ExchangeType { get; }
        public string RoutingKey { get; }
    }
}