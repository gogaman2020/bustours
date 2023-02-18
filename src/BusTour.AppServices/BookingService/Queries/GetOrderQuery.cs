using BusTour.Data.Repositories.Orders;
using BusTour.Domain.Entities;
using BusTour.Domain.Models.Filters;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusTour.AppServices.BookingService.Queries
{
    public class GetOrderQuery : MediatorQuery<Order>
    {
        private OrderFilter _orderFilter;

        private IOrderRepository _orderRepository;

        public GetOrderQuery(OrderFilter orderFilter)
        {
            _orderFilter = orderFilter;
            _orderRepository = IoC.GetRequiredService<IOrderRepository>();
        }

        public override async Task<MediatorCommandResult<Order>> ExecuteAsync()
        {
            return Success(await _orderRepository.FindAsync(_orderFilter));
        }
    }
}
