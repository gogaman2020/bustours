using BusTour.Data.Repositories.Orders;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusTour.AppServices.OrderService
{

    [InjectAsScoped]
    public class UpdateOrderSeatCommand : HighLevelMediatorCommand<OrderSeat>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderSeat _orderSeat;
        public UpdateOrderSeatCommand(OrderSeat orderSeat)
        {
            _orderRepository = IoC.GetRequiredService<IOrderRepository>();
            _orderSeat = orderSeat;
        }
        
        public override async Task<MediatorCommandResult<OrderSeat>> ExecuteAsync()
        {
            await _orderRepository.UpdateOrderSeat(_orderSeat);

            return Success(_orderSeat);
        }
    }
}
