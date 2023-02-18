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
    public sealed class CertificatePaymentFailCommand : HighLevelMediatorCommand<Payment>
    {
        private readonly int _certificateId;
        private readonly string _error;

        private readonly IPaymentRepository _paymentRepository;

        public CertificatePaymentFailCommand(int certificateId, string error = null)
        {
            _certificateId = certificateId;
            _error = error;

            _paymentRepository = IoC.GetRequiredService<IPaymentRepository>();
        }

        public async override Task<MediatorCommandResult<Payment>> ExecuteAsync()
        {
            var payment = new Payment
            {
                GiftCertificateId = _certificateId,
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
