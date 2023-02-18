using BusTour.AppServices.TourOrderProcess;
using BusTour.AppServices.TourOrderProcess.Args;
using BusTour.AppServices.TourOrderProcess.Commands;
using BusTour.AppServices.TourService;
using BusTour.Data.Repositories.GiftCertificates;
using BusTour.Data.Repositories.Orders;
using BusTour.Data.Repositories.Payments;
using BusTour.Data.Repositories.PromoCodes;
using BusTour.Data.Repositories.Tours;
using BusTour.Domain.Entities;
using BusTour.Domain.Models;
using BusTour.Domain.Models.Responses;
using Infrastructure.Common.DI;
using Infrastructure.Common.Extensions;
using Infrastructure.Mediator;
using Infrastructure.Process.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusTour.AppServices.Payments.Commands
{
    public sealed class OrderPaymentSuccessCommand : HighLevelMediatorCommand<Payment>
    {
        private readonly int _orderId;
        private readonly Client _client;

        private readonly ITourOrderProcess _process;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IGiftCertificateRepository _certificateRepository;
        private readonly IPromoCodeRepository _promoCodeRepository;

        public OrderPaymentSuccessCommand(int orderId, Client client)
        {
            _orderId = orderId;
            _client = client;

            _paymentRepository = IoC.GetRequiredService<IPaymentRepository>();
            _certificateRepository = IoC.GetRequiredService<IGiftCertificateRepository>();
            _orderRepository = IoC.GetRequiredService<IOrderRepository>();
            _process = IoC.GetRequiredService<ITourOrderProcess>();
            _tourRepository = IoC.GetRequiredService<ITourRepository>();
            _promoCodeRepository = IoC.GetRequiredService<IPromoCodeRepository>();
        }

        public async override Task<MediatorCommandResult<Payment>> ExecuteAsync()
        {
            var order = await _orderRepository.GetAsync(_orderId);

            if (order != null)
            {
                _process.Reset();
                await _process.SetContextAsync(_orderId);
                await _process.SendCommandAsync(new PayStepCommandArgs(TourOrderStepCommand.Payment) { IsPaid = true });

                var payment = new Payment
                {
                    OrderId = _orderId,
                    Details = new PaymentDetails
                    {
                        Error = null
                    }
                };

                payment.Id = await _paymentRepository.SaveOrUpdateAsync(payment);

                payment = await _paymentRepository.GetAsync(payment.Id);

                if (order.CertificateId.HasValue)
                {
                    var certificate = await _certificateRepository.GetAsync(order.CertificateId.Value);
                    certificate.RedeemedDate = DateTime.UtcNow;
                    certificate.RedeemedAmount = Math.Min(order.TotalPrice, certificate.Amount ?? 0);
                    await _certificateRepository.SaveOrUpdateAsync(certificate);
                }

                var tour = await _tourRepository.GetAsync(order.TourId);

                if (tour.Type == Domain.Enums.TourType.PrivateHire)
                {
                    await new TourCommandsHelpers().TryCancelCrossedTours(tour);
                }

                if (_client != null)
                {
                    if (!order.ClientId.HasValue)
                    {
                        order.Client = new Client();
                    }

                    order.Client.Email = _client.Email;
                    order.Client.PhoneNumber = _client.PhoneNumber;
                    order.Client.FullName = _client.FullName;

                    await _orderRepository.SaveOrUpdateAsync(order);
                }

                if (order.PromoCode?.PromoCodeType == Domain.Enums.PromoCodeType.ByDateAndAmount
                 || order.PromoCode?.PromoCodeType == Domain.Enums.PromoCodeType.ByAmount)
                {
                    var promocode = order.PromoCode;
                    promocode.NumberOfUses = (promocode.NumberOfUses ?? 0) + 1;

                    await _promoCodeRepository.SaveOrUpdateAsync(promocode);
                }

                return Success(payment);
            }
            else
            {
                return Fail();
            }
        }
    }
}
