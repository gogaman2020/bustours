using BusTour.AppServices.TourProcess.Args;
using Infrastructure.Process.Commands;
using System.Threading.Tasks;
using BusTour.AppServices.TourProcess.Steps;
using Infrastructure.Process.Args;
using BusTour.Domain.Models.Order;
using Infrastructure.Mediator;
using System.Collections.Generic;
using BusTour.Domain.Enums;
using BusTour.Data.Repositories.Orders;
using Infrastructure.Common.DI;
using BusTour.Data.Repositories.Tours;
using System.Linq;
using BusTour.Data.Repositories.BusRepository;
using BusTour.Domain.Entities;
using BusTour.Domain.Models.Filters;
using BusTour.Common;

namespace BusTour.AppServices.BookingService.Queries
{
    public class CheckOrderConflictsQuery : MediatorQuery<List<OrdersConflict>>
    {
        private int? _orderId;
        private Order _baseOrder;

        private IOrderRepository _orderRepository;
        private ITourRepository _tourRepository;
        private IBusRepository _busRepository;

        public CheckOrderConflictsQuery()
        {
            _orderRepository = IoC.GetRequiredService<IOrderRepository>();
            _tourRepository = IoC.GetRequiredService<ITourRepository>();
            _busRepository = IoC.GetRequiredService<IBusRepository>();
        }

        public CheckOrderConflictsQuery(OrderModel orderModel) : this()
        {
            _baseOrder = new Order
            {
                Id = orderModel.Id,
                IsGroup = orderModel.Type == OrderType.RegularGroup,
                TourId = orderModel.TourId,
                Seats = orderModel.Seats.Select(x => new OrderSeat { SeatId = x.SeatId }).ToList(),
                Tour = new Tour
                {
                    Type = orderModel.Type == OrderType.PrivateHire ? TourType.PrivateHire : TourType.Regular,
                    PrivateHire = new TourPrivateHire
                    {
                        BlockBookingDateFrom = orderModel.PrivateHire.BlockBookingDateTimeFrom,
                        BlockBookingDateTo = orderModel.PrivateHire.BlockBookingDateTimeTo
                    }
                }
            };
        }

        public CheckOrderConflictsQuery(int orderId) : this()
        {
            _orderId = orderId;
        }

        public CheckOrderConflictsQuery(Order order) : this()
        {
            _baseOrder = order;
        }

        public override async Task<MediatorCommandResult<List<OrdersConflict>>> ExecuteAsync()
        {
            if (_orderId.HasValue && _baseOrder == null)
            {
                _baseOrder = await _orderRepository.GetAsync(_orderId.Value);
            }
            return Success(await CheckOrder(_baseOrder));
        }

        private async Task<List<OrdersConflict>> CheckOrder(Order baseOrder)
        {
            var result = new List<OrdersConflict>();

            if (baseOrder.Type == OrderType.PrivateHire)
            {
                var tours = await _tourRepository.SelectAsync(new TourFilter
                {
                    DepartureDateTo = baseOrder.Tour.PrivateHire.BlockBookingDateTo,
                    ArrivalDateFrom = baseOrder.Tour.PrivateHire.BlockBookingDateFrom
                });

                var conflictOrders = await GetConflictOrders(tours.Select(x => x.Id));

                result = conflictOrders
                    .Select(order => new OrdersConflict(baseOrder, order, order.Seats.Select(x => x.SeatId)))
                    .ToList();
            }
            else if (baseOrder.Type <= OrderType.RegularGroup)
            {
                var baseOrderSeatIds = baseOrder.Seats.Select(x => x.SeatId).Distinct().ToList();

                //Конфликты по местам
                var conflictOrders = (await GetConflictOrders(new List<int> { baseOrder.TourId }))
                    .Where(x => x.Seats.Any(z => baseOrderSeatIds.Contains(z.SeatId))).ToList();

                //Конфликты по турам
                var tours = (await _tourRepository.SelectAsync(new TourFilter
                {
                    DepartureDateTo = baseOrder.Tour.Arrival,
                    ArrivalDateFrom = baseOrder.Tour.Departure
                })).Where(x => x.Type >= TourType.PrivateHire);

                conflictOrders.AddRange((await GetConflictOrders(tours.Select(x => x.Id))));

                result = conflictOrders
                    .Select(order => new OrdersConflict(baseOrder, order, order.Seats.Select(x => x.SeatId).Where(x => baseOrderSeatIds.Contains(x))))
                    .ToList();

            }

            return result;

        }

        private async Task<IEnumerable<Order>> GetConflictOrders(IEnumerable<int> tourIds)
        {
            var conflictOrders = (await _orderRepository.SelectAsync(new OrderFilter { TourIds = tourIds })).AsEnumerable();

            conflictOrders = conflictOrders.Where(x => x.OrderState < OrderState.Canceled);

            if (_baseOrder.Id != default(int))
            {
                conflictOrders = conflictOrders.Where(x => x.Id != _baseOrder.Id);
            }

            return conflictOrders.DistinctBy(x => x.Id);
        }
    }
}
