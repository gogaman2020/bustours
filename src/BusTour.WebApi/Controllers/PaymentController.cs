using BusTour.AppServices.BookingService;
using BusTour.AppServices.BookingService.Commands;
using BusTour.AppServices.OrderService;
using BusTour.AppServices.BookingService.Queries;
using BusTour.Domain.Entities;
using BusTour.Domain.Models;
using BusTour.Domain.Models.Filters;
using BusTour.Domain.Models.Order;
using BusTour.Domain.Models.Responses;
using Infrastructure.Common.DI;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using BusTour.AppServices.Payments.Commands;
using System.Collections.Generic;
using BusTour.Domain.Enums;

namespace BusTour.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [InjectAsSingleton]
    public class PaymentController : BusTourControllerBase
    {
        public PaymentController() : base()
        {
        }

        [HttpGet]
        [Route("GetWaitingForPaymentOrders")]
        public async Task<List<int>> GetWaitingForPaymentOrders()
        {
            var result = (await RunCommandAsync(new GetOrdersQuery(new OrderFilter { States = new List<OrderState> { OrderState.WaitingForPayment } })));
            
            var orders = (result.Result as dynamic).Value as List<Order>;

            return orders.Select(x => x.Id).ToList();
        }

        [HttpPost]
        [Route("OrderPaymentSuccess")]
        public async Task<ActionResult<Payment>> OrderPaymentSuccess(PaymentModel payment)
        {
            //var res = await RunCommandAsync(new AddNotificationCommand(payment.OrderId, payment.Client?.Email, ""));
            return await RunCommandAsync(new OrderPaymentSuccessCommand(payment.OrderId, payment.Client));
        }

        [HttpPost]
        [Route("OrderPaymentFail")]
        public async Task<ActionResult<Payment>> OrderPaymentFail(PaymentModel payment)
        {
            return await RunCommandAsync(new OrderPaymentFailCommand(payment.OrderId, payment.Error));
        }

        [HttpPost]
        [Route("CertificatePaymentSuccess")]
        public async Task<ActionResult<Payment>> CertificatePaymentSuccess(PaymentModel payment)
        {
            return await RunCommandAsync(new CertificatePaymentSuccessCommand(payment.CertificateId));
        }

        [HttpPost]
        [Route("CertificatePaymentFail")]
        public async Task<ActionResult<Payment>> CertificatePaymentFail(PaymentModel payment)
        {
            return await RunCommandAsync(new CertificatePaymentFailCommand(payment.CertificateId, payment.Error));
        }
    }

    public class PaymentModel
    {
        public int OrderId { get; set; }
        public int CertificateId { get; set; }
        public string Error { get; set; }
        public Client Client { get; set; }
    }
}
