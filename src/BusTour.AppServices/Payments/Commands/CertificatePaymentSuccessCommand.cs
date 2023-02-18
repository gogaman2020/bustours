using BusTour.AppServices.TourOrderProcess;
using BusTour.AppServices.TourOrderProcess.Args;
using BusTour.AppServices.TourOrderProcess.Commands;
using BusTour.Data.Repositories.GiftCertificates;
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
    public sealed class CertificatePaymentSuccessCommand : HighLevelMediatorCommand<Payment>
    {
        private readonly int _certificateId;

        private readonly IPaymentRepository _paymentRepository;

        private readonly IGiftCertificateRepository _certificateRepository;

        public CertificatePaymentSuccessCommand(int certificateId)
        {
            _certificateId = certificateId;

            _paymentRepository     = IoC.GetRequiredService<IPaymentRepository>();
            _certificateRepository = IoC.GetRequiredService<IGiftCertificateRepository>();
        }

        public async override Task<MediatorCommandResult<Payment>> ExecuteAsync()
        {
            var certificate = await _certificateRepository.GetAsync(_certificateId);

            if (certificate != null)
            {
                certificate.IsPaid = true;
                await _certificateRepository.SaveOrUpdateAsync(certificate);

                var payment = new Payment
                {
                    GiftCertificateId = _certificateId,
                    Details = new PaymentDetails
                    {
                        Error = null
                    }
                };

                payment.Id = await _paymentRepository.SaveOrUpdateAsync(payment);

                payment = await _paymentRepository.GetAsync(payment.Id);

                return Success(payment);
            }
            else
            {
                return Fail();
            }
        }
    }
}
