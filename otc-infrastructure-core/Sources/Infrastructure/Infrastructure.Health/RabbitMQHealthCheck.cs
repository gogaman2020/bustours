using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Health
{
    public class RabbitMQHealthCheck : IHealthCheck
    {
        private IConnectionFactory _factory;

        public RabbitMQHealthCheck(string host, string virtualHost, string user, string password)
        {
            _factory = new ConnectionFactory()
            {
                HostName = host,
                VirtualHost = virtualHost,
                UserName = user,
                Password = password,
                DispatchConsumersAsync = true
            };
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var connection = _factory.CreateConnection())
                using (connection.CreateModel())
                {
                    return Task.FromResult(
                        HealthCheckResult.Healthy());
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult(
                    new HealthCheckResult(context.Registration.FailureStatus, exception: ex));
            }
        }
    }
}
