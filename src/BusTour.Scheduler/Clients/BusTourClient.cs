using BusTour.Scheduler.Options;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace BusTour.Scheduler.Clients
{
    public class BusTourClient : ClientBase, IBusTourClient
    {
        public BusTourClient(IOptions<ClientsConfig> clientsConfig)
            : base(clientsConfig.Value.BusTourApiUrl)
        {
        }

        public Task CancelUnpaidOrdersAsync()
        {
            return _client.PostAsync<object>("/Jobs/cancel-unpaid-orders", null);
        }

        public Task SendNotifications()
        {
            return _client.PostAsync<object>("/Jobs/send-notifications", null);
        }
    }
}
