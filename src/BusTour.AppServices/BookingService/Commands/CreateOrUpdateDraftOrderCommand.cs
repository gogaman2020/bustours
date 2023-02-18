using BusTour.AppServices.TourOrderProcess;
using BusTour.AppServices.TourOrderProcess.Commands;
using BusTour.AppServices.TourOrderProcess.Steps;
using BusTour.AppServices.BookingService.Queries;
using BusTour.Data.Repositories.Orders;
using BusTour.Data.Repositories.Tours;
using BusTour.Domain.Entities;
using BusTour.Domain.Models.Order;
using BusTour.Domain.Models.Responses;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusTour.AppServices.TourService;
using BusTour.Data.Repositories.BusRepository;
using BusTour.Common;
using BusTour.AppServices.Notifications;
using BusTour.Domain.Models.NotificationEvents;
using BusTour.Domain.Models.Filters;
using BusTour.AppServices.OrderService;

namespace BusTour.AppServices.BookingService.Commands
{
    public sealed class CreateOrUpdateDraftOrderCommand : HighLevelMediatorCommand<OrderResponse>
    {
        private readonly OrderModel _orderModel;

        private readonly IBookingService _bookingService;
        private readonly IOrderRepository _orderRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IBusRepository _busRepository;
        private readonly ITourOrderProcess _process;
        private readonly ITourService _tourService;
        private readonly INotificationServiсe _notificationServiсe;

        public CreateOrUpdateDraftOrderCommand(OrderModel orderModel)
        {
            _orderModel = orderModel;
            _bookingService = IoC.GetRequiredService<IBookingService>();
            _orderRepository = IoC.GetRequiredService<IOrderRepository>();
            _tourRepository = IoC.GetRequiredService<ITourRepository>();
            _process = IoC.GetRequiredService<ITourOrderProcess>();
            _tourService = IoC.GetRequiredService<ITourService>();
            _busRepository = IoC.GetRequiredService<IBusRepository>();
            _notificationServiсe = IoC.GetRequiredService<INotificationServiсe>();
        }

        public override async Task<MediatorCommandResult<OrderResponse>> ExecuteAsync()
        {
            if (_orderModel == null)
            {
                return Fail("Order model is empty.");
            }

            if (await _tourRepository.GetAsync(_orderModel.TourId) == null)
            {
                return Fail("Tour not found.");
            }

            var order = _bookingService.ConvertToEntity(_orderModel);

            if (_orderModel.Id != 0)
            {
                if (await _orderRepository.GetAsync(order.Id) == null)
                {
                    return Fail("Order not found.");
                }

                await _orderRepository.DeleteNestedAsync(order);
            }

            //var conflicts = (await Mediator.RunCommandAsync(new CheckOrderConflictsQuery(_orderModel))).Result;

            //if (conflicts.Any(x => x.IsBlocking))
            //{
            //    return Fail("There exist blocking conflicts: " + string.Join(",", conflicts.Where(x => x.IsBlocking).Select(x => x.ConflictOrder.Id)));
            //}

            order.TotalPrice = (await Mediator.RunCommandAsync(new GetCalculationCostTourQuery(_orderModel))).Result.TotalPrice;

            order.Id = await _orderRepository.SaveOrUpdateAsync(order);

            order = await _orderRepository.GetAsync(order.Id);

            if (order.IsGroup)
            {
                await _notificationServiсe.AddNotificationAsync(new GroupOrderCreatedNotificationEvent(order));
            }
            else
            {
                await _notificationServiсe.AddNotificationAsync(new RegularOrderCreatedNotificationEvent(order));
            }

            var result = new OrderResponse
            {
                IsSuccess = true,
                OrderId = order.Id,
                Hash = order.Hash
            };

            if (_orderModel.Id == 0)
            {
                _process.Reset();
                await _process.SetContextAsync(order);
                await _process.InitStateAsync();
            }

            return Success(result);
        }
    }
}
