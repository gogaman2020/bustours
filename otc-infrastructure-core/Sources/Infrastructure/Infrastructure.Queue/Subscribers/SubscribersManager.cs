using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Common.DI;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Queue.Subscribers
{
    [InjectAsSingleton]
    public class SubscribersManager : IHostedService
    {
        private readonly List<Func<IDisposable>> _subscriberFactories = new List<Func<IDisposable>>();
        private readonly List<IDisposable> _subscribers = new List<IDisposable>();
        private readonly ILogger<SubscribersManager> _logger;

        public SubscribersManager(ILogger<SubscribersManager> logger)
        {
            _logger = logger;
        }

        public void Add(Func<IDisposable> action)
        {
            _subscriberFactories.Add(action);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var factory in _subscriberFactories)
            {
                try
                {
                    _subscribers.Add(factory.Invoke());
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Exception on start subscriber");
                }
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var subscriber in _subscribers)
            {
                try
                {
                    subscriber.Dispose();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Exception on start subscriber");
                }
            }
            _subscribers.Clear();
            return Task.CompletedTask;
        }
    }
}