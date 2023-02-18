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
    public class CheckOrdersConflictsQuery : MediatorQuery<List<OrdersConflictWithoutBase>>
    {
        private OrderFilter _orderFilter;

        private IOrderRepository _orderRepository;

        public CheckOrdersConflictsQuery()
        {
            _orderRepository = IoC.GetRequiredService<IOrderRepository>();
        }
        public CheckOrdersConflictsQuery(OrderFilter orderFilter) : this()
        {
            _orderFilter = orderFilter;
        }

        public override async Task<MediatorCommandResult<List<OrdersConflictWithoutBase>>> ExecuteAsync()
        {
            var allConflicts = new List<OrdersConflict>();

            if (_orderFilter != null)
            {
                _orderFilter.States = new List<OrderState> { OrderState.Paid, OrderState.WaitingForPayment, OrderState.Draft };
                
                var orders = await _orderRepository.SelectAsync(_orderFilter);

                foreach (var order in orders)
                {
                    var orderConflicts = (await Mediator.RunCommandAsync(new CheckOrderConflictsQuery(order))).Result;
                    var conflictSeatIds = orderConflicts.SelectMany(x => x.ConflictSeatIds).Distinct();
                    if (orderConflicts.Any())
                    {
                        orderConflicts.Add(new OrdersConflict(order, order, conflictSeatIds));
                    }
                    allConflicts.AddRange(orderConflicts);
                }
            }

            var allOrders = allConflicts.Select(x => x.ConflictOrder).DistinctBy(x => x.Id);

            var groups = allConflicts.GroupBy(x => x.ConflictOrder.Id).Select(x => new { 
                orderId = x.Key, 
                seatIds = x.SelectMany(z => z.ConflictSeatIds).Distinct()
            });

            return Success(groups.Select(x => new OrdersConflictWithoutBase(allOrders.First(o => o.Id == x.orderId), x.seatIds)).OrderBy(x => x.ConflictOrder.Id).ToList());
        }
    }
}
