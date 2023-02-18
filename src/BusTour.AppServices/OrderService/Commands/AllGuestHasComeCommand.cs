using BusTour.Data.Repositories.Orders;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusTour.AppServices.OrderService
{
    [InjectAsScoped]
    public class AllGuestHasComeCommand : HighLevelMediatorCommand<bool>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly int _orderId;
        public AllGuestHasComeCommand(int orderId)
        {
            _orderRepository = IoC.GetRequiredService<IOrderRepository>();
            _orderId = orderId;
        }

        public override async Task<MediatorCommandResult<bool>> ExecuteAsync()
        {
            if (await _orderRepository.GetAsync(_orderId) == null)
            {
                return Fail($"Order with id: {_orderId} not found");
            }
            await _orderRepository.AllGuestHasComeAsync(_orderId);

            return Success(true);
        }
    }
}
