using BusTour.AppServices.TourProcess.Args;
using Infrastructure.Process.Commands;
using System.Threading.Tasks;
using BusTour.AppServices.TourProcess.Steps;
using Infrastructure.Process.Args;
using BusTour.Domain.Models.Order;
using Infrastructure.Mediator;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using BusTour.Data.Repositories.Tours;
using BusTour.Data.Repositories.Orders;
using BusTour.AppServices.BookingService;
using BusTour.Domain.Enums;
using BusTour.AppServices.BookingService.Queries;
using System.Linq;
using BusTour.Common;
using BusTour.AppServices.TourOrderProcess;
using BusTour.AppServices.TourOrderProcess.Commands;
using BusTour.Domain.Models;
using BusTour.Data.Repositories.BusRepository;
using BusTour.Domain.Models.Responses;
using BusTour.AppServices.TourProcess;
using BusTour.AppServices.TourProcess.Commands;
using BusTour.AppServices.TourService.Queries;
using BusTour.Domain.Models.Filters;
using System.Collections.Generic;
using BusTour.AppServices.TourService.Commands;
using BusTour.AppServices.TourService;
using BusTour.Domain.Models.NotificationEvents;
using BusTour.AppServices.Notifications;

namespace BusTour.AppServices.BookingService.Commands
{
    public sealed class CreateOrUpdatePrivateHireOrderCommand : HighLevelMediatorCommand<Order, OrderCreationFailResponse>
    { 

        private OrderModel _orderModel;

        public CreateOrUpdatePrivateHireOrderCommand(OrderModel orderModel)
        {
            _orderModel = orderModel;
        }

        public override async Task<MediatorCommandResult<Order>> ExecuteAsync()
        {
            var tourRepository  = IoC.GetRequiredService<ITourRepository>();
            var orderRepository = IoC.GetRequiredService<IOrderRepository>();
            var bookingService  = IoC.GetRequiredService<IBookingService>();
            var busRepository   = IoC.GetRequiredService<IBusRepository>();
            var orderProcess    = IoC.GetRequiredService<ITourOrderProcess>();
            var tourProcess     = IoC.GetRequiredService<ITourProcess>();
            var notificationServiсe = IoC.GetRequiredService<INotificationServiсe>();

            _orderModel.PrivateHire.BusId = (await busRepository.GetBusesAsync()).First().Id;

            var conflicts = (await Mediator.RunCommandAsync(new CheckOrderConflictsQuery(_orderModel))).Result;

            if (conflicts.Any(x => x.IsBlocking))
            {
                return Fail(new OrderCreationFailResponse
                {
                    BlockingConflicts = conflicts.Where(x => x.IsBlocking)
                });
            }

            var privateHire = _orderModel.PrivateHire;

            Tour tour = null;
            if (_orderModel.TourId != default(int))
            {
                tour = await tourRepository.GetAsync(_orderModel.TourId);
            }
            if (tour == null)
            {
                tour = new Tour();
            }

            tour.BusId = privateHire.BusId;
            tour.Departure = privateHire.DepartureDateTime;
            tour.RouteId = privateHire.RouteId is default(int) ? null : privateHire.RouteId;
            tour.Type = TourType.PrivateHire;

            if (tour.PrivateHire == null)
            {
                tour.PrivateHire = new TourPrivateHire();
            }

            tour.PrivateHire.Duration = privateHire.ArrivalDateTime - privateHire.DepartureDateTime;
            tour.PrivateHire.BlockBookingDateFrom = privateHire.BlockBookingDateTimeFrom;
            tour.PrivateHire.BlockBookingDateTo = privateHire.BlockBookingDateTimeTo;
            tour.PrivateHire.BlockBookingForDraft = privateHire.BlockBookingForDraft;
            tour.PrivateHire.DeparturePoint = privateHire.DeparturePoint;
            tour.PrivateHire.ArrivalPoint = privateHire.ArrivalPoint;
            tour.PrivateHire.Stops = privateHire.Stops;
            tour.PrivateHire.Price = privateHire.TourPrice;
            tour.PrivateHire.GuestCount = _orderModel.GuestCount;

            _orderModel.TourId = await tourRepository.SaveOrUpdateAsync(tour);
            tour = await tourRepository.GetAsync(_orderModel.TourId);

            if (!tour.TourState.HasValue)
            {
                tourProcess.Reset();
                await tourProcess.SetContextAsync(tour.Id);
                await tourProcess.InitStateAsync();
                await tourProcess.SendCommandAsync(TourStepCommand.Active);
            }

            var order = bookingService.ConvertToEntity(_orderModel);
            await orderRepository.SaveOrUpdateAsync(order);
            order = await orderRepository.GetAsync(order.Id);

            if (!order.OrderState.HasValue)
            {
                orderProcess.Reset();
                await orderProcess.SetContextAsync(order.Id);
                await orderProcess.InitStateAsync();
                await orderProcess.SendCommandAsync(TourOrderStepCommand.WaitingForPaiment);
            }

            if (privateHire.BlockBookingForDraft)
            {
                await new TourCommandsHelpers().TryCancelCrossedTours(tour);
            }

            await notificationServiсe.AddNotificationAsync(new PrivateHireOrderCreatedNotificationEvent(order));

            return Success(order);
        }

    }
}
