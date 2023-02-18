using System.Threading.Tasks;

namespace BusTour.Scheduler.Clients
{
    public interface IBusTourClient
    {
        Task CancelUnpaidOrdersAsync();
        Task SendNotifications();
    }
}
