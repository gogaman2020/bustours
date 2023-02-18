using System;
using Infrastructure.Queue.Configs.Old;
using RabbitMQ.Client;

namespace Infrastructure.Queue.Subscribers
{
    public interface ISubscriberFactory
    {
        IDisposable Subscribe<T>(ISubscriber<T> subscriber)
            where T : class;

        IDisposable Subscribe(ITopologyConfiguration configuration, IBasicConsumer consumer, string queueName);
    }
}
