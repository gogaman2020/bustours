using BusTour.AppServices.TourOrderProcess;
using BusTour.AppServices.TourOrderProcess.Args;
using BusTour.AppServices.TourOrderProcess.Commands;
using BusTour.Data.Repositories.Payments;
using BusTour.Domain.Entities;
using BusTour.Domain.Models;
using BusTour.Domain.Models.Responses;
using Infrastructure.Common.DI;
using Infrastructure.Common.Extensions;
using Infrastructure.Mediator;
using Infrastructure.Process.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusTour.AppServices.Payments.Commands
{
    public sealed class OrderPaymentFailCommand : HighLevelMediatorCommand<Payment>
    {
        private readonly int _orderId;
        private readonly string _error;

        private readonly ITourOrderProcess _process;
        private readonly IPaymentRepository _paymentRepository;

        public OrderPaymentFailCommand(int orderId, string error = null)
        {
            _orderId = orderId;
            _error = error;

            _paymentRepository = IoC.GetRequiredService<IPaymentRepository>();
            _process = IoC.GetRequiredService<ITourOrderProcess>();
        }

        public async override Task<MediatorCommandResult<Payment>> ExecuteAsync()
        {
            //_process.Reset();
            //await _process.SetContextAsync(_orderId);
            //await _process.SendCommandAsync(new PayStepCommandArgs(TourOrderStepCommand.Payment) { IsPaid = false });

            var payment = new Payment
            {
                OrderId = _orderId,
                Details = new PaymentDetails
                {
                    Error = _error
                }
            };

            payment.Id = await _paymentRepository.SaveOrUpdateAsync(payment);

            payment = await _paymentRepository.GetAsync(payment.Id);

            return Success(payment);
        }
    }
}
