using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System.Threading.Tasks;

namespace BusTour.AppServices.Notifications.Commands
{
    [InjectAsScoped]
    public class SendNotificationsCommand : HighLevelMediatorCommand<bool>
    {
        private readonly INotificationServiсe _notificationServiсe;

        public SendNotificationsCommand()
        {
            _notificationServiсe = IoC.GetRequiredService<INotificationServiсe>();
        }

        public override async Task<MediatorCommandResult<bool>> ExecuteAsync()
        {
            var notifications = await _notificationServiсe.GetNotificationsToSendAsync();

            foreach(var notification in notifications)
            {
                await _notificationServiсe.SendNotificationAsync(notification);
            }

            return Success(true);
        }
    }
}