using BusTour.AppServices.TourOrderProcess;
using BusTour.AppServices.TourOrderProcess.Commands;
using BusTour.AppServices.TourProcess;
using BusTour.AppServices.TourProcess.Commands;
using BusTour.Common.Config;
using BusTour.Data.Repositories.Orders;
using BusTour.Domain.Models.Filters;
using BusTour.Domain.Models.Responses;
using Infrastructure.Common.Configs;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System;
using System.Threading.Tasks;

namespace BusTour.AppServices.Notifications.Commands
{
    public class ContactUsCommand : HighLevelMediatorCommand<BaseResponse>
    {
        public string Phone { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }

        private readonly INotificationServiсe _notificationServiсe;
        private readonly ApiConfig _apiConfig;

        public ContactUsCommand()
        {
            _notificationServiсe = IoC.GetRequiredService<INotificationServiсe>();
            _apiConfig = Config.Get<ApiConfig>();
        }

        public override async Task<MediatorCommandResult<BaseResponse>> ExecuteAsync()
        {
            try
            {
                await _notificationServiсe.SendEmailAsync(
                    _apiConfig.AdminEmail, 
                    "Contact us", 
                    $"Subject: {Subject}<br><br>Text: {Text}<br><br>Phone: {Phone}"
                    );

                return Success(new BaseResponse { IsSuccess = true });
            }
            catch (Exception exception)
            {
                return Fail(exception.Message);
            }
        }
    }
}
