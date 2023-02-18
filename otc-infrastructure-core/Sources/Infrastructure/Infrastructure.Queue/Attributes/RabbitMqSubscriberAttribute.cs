using System;

namespace Infrastructure.Queue.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RabbitMqSubscriberAttribute : Attribute
    {
        public RabbitMqSubscriberAttribute(string exchangeName = null, string queueName = null, bool useDlx = false,
            int ttl = 300000, int retryCount = 10, int limit = 1, string configName = null)
        {
            ExchangeName = exchangeName;
            QueueName = queueName;
            UseDlx = useDlx;
            Ttl = ttl;
            RetryCount = retryCount;
            Limit = limit;
            ConfigName = configName;
        }

        public string ConfigName { get; }

        public bool UseDlx { get; }

        public int Ttl { get; }

        public int RetryCount { get; }

        public int Limit { get; }

        public string ExchangeName { get; }

        public string QueueName { get; }
    }
}