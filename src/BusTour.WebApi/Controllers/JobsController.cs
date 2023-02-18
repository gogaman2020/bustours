using BusTour.AppServices.GiftCertificates.Queries;
using BusTour.AppServices.Notifications;
using BusTour.AppServices.Notifications.Commands;
using BusTour.AppServices.TourService.Commands;
using BusTour.Domain.Entities;
using BusTour.Domain.Models.Responses;
using Infrastructure.Common.DI;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BusTour.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [InjectAsSingleton]
    public class JobsController : BusTourControllerBase
    {
        private INotificationServiсe _notificationServiсe;
        public JobsController(INotificationServiсe notificationServiсe)
        {
            _notificationServiсe = notificationServiсe;
        }

        [HttpPost]
        [Route("cancel-unpaid-orders")]
        public async Task<ActionResult<List<int>>> CancelUnpaidOrders()
        {
            return await RunCommandAsync(new CancelUnpaidOrdersCommand());
        }

        [HttpPost]
        [Route("send-notifications")]
        public async Task<ActionResult<bool>> SendNotifications()
        {
            var notifications = await _notificationServiсe.GetNotificationsToSendAsync();

            foreach (var notification in notifications)
            {
                await _notificationServiсe.SendNotificationAsync(notification);
            }

            return new ActionResult<bool>(true);
        }

        [HttpGet("generate-pdf")]
        public async Task<ActionResult> AppsUsagePdf(int id)
        {
            var result = new GetCertificatePdfCommand(id).ExecuteAsync().Result.Result;

            var stream = new MemoryStream(result);

            return new FileStreamResult(stream, "application/pdf");
        }

        [HttpGet("test")]
        public ActionResult test()
        {
            return Ok("stable");
        }

        [HttpPost]
        [Route("contactUs")]
        public Task<ActionResult<BaseResponse>> ContactUs(ContactUsCommand command)
        {
            return RunCommandAsync(command);
        }
    }
}
