using BusTour.Scheduler.Clients;
using BusTour.Scheduler.Helpers;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading.Tasks;

namespace BusTour.Scheduler.Jobs
{
    [DisallowConcurrentExecution]
    [InjectAsSingleton]
    public class CancelUnpaidOrdersJob : IJob
    {
        private readonly ILogger<CancelUnpaidOrdersJob> _logger;

        private readonly IBusTourClient _busTourClient;

        public CancelUnpaidOrdersJob(
            ILogger<CancelUnpaidOrdersJob> logger,
            IBusTourClient busTourClient)
        {
            _logger = logger;

            _busTourClient = busTourClient;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogDebug("CancelUnpaidOrders");

            await _busTourClient.CancelUnpaidOrdersAsync();
        }
    }
}
