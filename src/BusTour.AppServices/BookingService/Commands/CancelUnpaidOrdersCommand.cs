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
using Infrastructure.Common.Configs;
using BusTour.Common.Config;

namespace BusTour.AppServices.TourService.Commands
{
    [InjectAsScoped]
    public class CancelUnpaidOrdersCommand : HighLevelMediatorCommand<List<int>>
    {
        private readonly ITourOrderProcess _orderProcess;
        private readonly ITourProcess _tourProcess;
        private readonly IOrderRepository _orderRepository;
        private readonly ApiConfig _apiConfig;

        public CancelUnpaidOrdersCommand()
        {
            _orderProcess = IoC.GetRequiredService<ITourOrderProcess>();
            _tourProcess = IoC.GetRequiredService<ITourProcess>();
            _orderRepository = IoC.GetRequiredService<IOrderRepository>();
            _apiConfig = Config.Get<ApiConfig>();
        }

        public override async Task<MediatorCommandResult<List<int>>> ExecuteAsync()
        {
            var orders = await _orderRepository.SelectAsync(new OrderFilter 
            { 
                States = new List<OrderState> { OrderState.WaitingForPayment, OrderState.Draft } 
            });

            foreach(var order in orders.Where(x => x.OrderDate.AddMinutes(_apiConfig.PaymentMinutes) < DateTime.UtcNow))
            {
                await _orderProcess.SendCommandAsync(order.Id, TourStepCommand.Cancel);
            }

            await new TourCommandsHelpers().TryCancelCancelRequestsTours();

            return Success(orders.Select(x => x.Id).ToList());
        }
    }
}