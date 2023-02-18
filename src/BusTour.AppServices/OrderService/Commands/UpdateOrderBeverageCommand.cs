using BusTour.Data.Repositories.Orders;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusTour.AppServices.OrderService
{
    [InjectAsScoped]
    public class UpdateOrderBeverageCommand : HighLevelMediatorCommand<OrderBeverage>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderBeverage _orderBeverage;
        public UpdateOrderBeverageCommand(OrderBeverage orderBeverage)
        {
            _orderRepository = IoC.GetRequiredService<IOrderRepository>();
            _orderBeverage = orderBeverage;
        }

        public override async Task<MediatorCommandResult<OrderBeverage>> ExecuteAsync()
        {
            await _orderRepository.UpdateOrderBeverage(_orderBeverage);

            return Success(_orderBeverage);
        }
    }
}
