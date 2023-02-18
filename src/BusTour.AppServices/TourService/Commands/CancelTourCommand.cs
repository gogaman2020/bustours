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
using BusTour.AppServices.TourService.Queries;
using BusTour.AppServices.TourOrderProcess.Commands;
using BusTour.Data.Repositories.GiftCertificates;

namespace BusTour.AppServices.TourService.Commands
{
    [InjectAsScoped]
    public class CancelTourCommand : HighLevelMediatorCommand<bool, CancelTourCommand.CancelTourError>
    {
        private readonly ITourRepository _tourRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ITourProcess _tourProcess;
        private readonly ITourOrderProcess _orderProcess;
        private readonly IGiftCertificateRepository _certificateRepository;

        public CancelTourCommand()
        {
            _tourRepository = IoC.GetRequiredService<ITourRepository>();
            _orderRepository = IoC.GetRequiredService<IOrderRepository>();
            _tourProcess = IoC.GetRequiredService<ITourProcess>();
            _orderProcess = IoC.GetRequiredService<ITourOrderProcess>();
            _certificateRepository = IoC.GetRequiredService<IGiftCertificateRepository>();
        }

        public override async Task<MediatorCommandResult<bool>> ExecuteAsync()
        {
            if (int.TryParse(Id, out int tourId))
            {
                var canBeCancelled = (await Mediator.RunCommandAsync(new CheckTourCanBeCancelledQuery(tourId))).Result;

                 if (!canBeCancelled)
                {
                    return Fail(new CancelTourCommand.CancelTourError { HasPaidOrders = true });
                }

                await _tourProcess.SendCommandAsync(tourId, TourStepCommand.Cancel);

                var tourOrders = await _orderRepository.SelectAsync(new OrderFilter
                {
                    TourIds = new List<int> { tourId }
                });

                foreach(var tourOrder in tourOrders.Where(x => x.OrderState <= OrderState.WaitingForPayment))
                {
                    await _orderProcess.SendCommandAsync(tourOrder.Id, TourOrderStepCommand.Cancel);

                    var order = await _orderRepository.GetAsync(tourOrder.Id);

                    if (order.CertificateId.HasValue)
                    {
                        var cetrificate = await _certificateRepository.GetAsync(order.CertificateId.Value);
                        cetrificate.RedeemedAmount = null;
                        cetrificate.RedeemedDate = null;
                        await _certificateRepository.SaveOrUpdateAsync(cetrificate);

                        order.CertificateId = null;
                        await _orderRepository.SaveOrUpdateAsync(order);
                    }
                }

                return Success(true);
            }
            else
            {
                return Fail();
            }
        }

        public class CancelTourError
        {
            public bool HasPaidOrders { get; set; }
        }
    }
}