using System;
using System.Threading;
using System.Threading.Tasks;
using ClientsCommon;
using Infrastructure.Common.DI;
using Infrastructure.Common.Helpers;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Infrastructure.Health
{
    public class ClientCheck<T> : IHealthCheck
        where T : ICommonClient
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = IoC.GetService<T>();
                await client.PingAsync();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(exception: ex);
            }

            return HealthCheckResult.Healthy();
        }
    }
}
