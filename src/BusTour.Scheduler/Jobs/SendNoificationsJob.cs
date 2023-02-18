using BusTour.Scheduler.Clients;
using BusTour.Scheduler.Helpers;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading.Tasks;

namespace BusTour.Scheduler.Jobs
{
    [DisallowConcurrentExecution]
    [InjectAsSingleton]
    public class SendNoificationsJob : IJob
    {
        private readonly ILogger<SendNoificationsJob> _logger;

        private readonly IBusTourClient _busTourClient;

        public SendNoificationsJob(
            ILogger<SendNoificationsJob> logger,
            IBusTourClient busTourClient)
        {
            _logger = logger;
            _busTourClient = busTourClient;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogDebug("SendNoificationsJob");

            await _busTourClient.SendNotifications();
        }
    }
}
