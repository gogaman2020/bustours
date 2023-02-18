using BusTour.AppServices.TourProcess;
using BusTour.Data.Repositories.Tours;
using BusTour.Data.Repositories.Users;
using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using BusTour.Domain.Extensions;
using BusTour.Domain.Models;
using BusTour.Domain.Models.Auth;
using Infrastructure.Common.DI;
using Infrastructure.Common.Context;
using Infrastructure.Mediator;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BusTour.Data.Repositories.Orders;
using BusTour.AppServices.TourOrderProcess;
using BusTour.Domain.Models.Filters;
using BusTour.AppServices.TourProcess.Commands;
using BusTour.AppServices.TourOrderProcess.Commands;
using BusTour.AppServices.TourService.Queries;
using BusTour.AppServices.Notifications;
using BusTour.Domain.Models.NotificationEvents;
using BusTour.Data.Repositories.GiftCertificates;
using BusTour.Data.Repositories.PromoCodes;

namespace BusTour.AppServices.TourService.Commands
{
    [InjectAsScoped]
    public class CancelOrderCommand : HighLevelMediatorCommand<bool, CancelOrderCommand.CancelOrderError>
    {
        private readonly ITourOrderProcess _orderProcess;
        private readonly ITourProcess _tourProcess;
        private readonly IOrderRepository _orderRepository;
        private readonly IGiftCertificateRepository _certificateRepository;
        private readonly INotificationServiсe _notificationServiсe;
        private readonly IPromoCodeRepository _promoCodeRepository;

        public CancelOrderCommand()
        {
            _orderProcess = IoC.GetRequiredService<ITourOrderProcess>();
            _tourProcess = IoC.GetRequiredService<ITourProcess>();
            _orderRepository = IoC.GetRequiredService<IOrderRepository>();
            _notificationServiсe = IoC.GetRequiredService<INotificationServiсe>();
            _certificateRepository = IoC.GetRequiredService<IGiftCertificateRepository>();
            _promoCodeRepository = IoC.GetRequiredService<IPromoCodeRepository>();
        }

        public override async Task<MediatorCommandResult<bool>> ExecuteAsync()
        {
            if (int.TryParse(Id, out int orderId))
            {
                await _orderProcess.SendCommandAsync(orderId, TourOrderStepCommand.Cancel);

                var order = await _orderRepository.GetAsync(orderId);

                if (order.Tour.TourState == TourState.CancelRequest)
                {
                    if ((await Mediator.RunCommandAsync(new CheckTourCanBeCancelledQuery(order.TourId))).Result)
                    {
                        await _tourProcess.SendCommandAsync(order.TourId, TourStepCommand.Cancel);
                    }
                }

                if (order.CertificateId.HasValue)
                {
                    var cetrificate = await _certificateRepository.GetAsync(order.CertificateId.Value);
                    cetrificate.RedeemedAmount = null;
                    cetrificate.RedeemedDate   = null;
                    await _certificateRepository.SaveOrUpdateAsync(cetrificate);

                    order.CertificateId = null;
                    await _orderRepository.SaveOrUpdateAsync(order);
                }

                if (order.PromoCode?.PromoCodeType == Domain.Enums.PromoCodeType.ByDateAndAmount
                 || order.PromoCode?.PromoCodeType == Domain.Enums.PromoCodeType.ByAmount)
                {
                    var promocode = order.PromoCode;
                    promocode.NumberOfUses = promocode.NumberOfUses > 0 ? promocode.NumberOfUses - 1 : 0;

                    await _promoCodeRepository.SaveOrUpdateAsync(promocode);
                }

                await _notificationServiсe.AddNotificationAsync(new CancelOrderCreatedNotificationEvent(order));

                return Success(true);
            }
            else
            {
                return Fail();
            }
        }

        public class CancelOrderError
        {
        }
    }
}