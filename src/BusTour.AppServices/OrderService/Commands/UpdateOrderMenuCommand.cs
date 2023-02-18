using BusTour.Data.Repositories.Orders;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System.Threading.Tasks;

namespace BusTour.AppServices.OrderService
{
    [InjectAsScoped]
    public class UpdateOrderMenuCommand : HighLevelMediatorCommand<OrderMenu>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderMenu _orderMenu;
        public UpdateOrderMenuCommand(OrderMenu orderMenu)
        {
            _orderRepository = IoC.GetRequiredService<IOrderRepository>();
            _orderMenu = orderMenu;
        }

        public override async Task<MediatorCommandResult<OrderMenu>> ExecuteAsync()
        {
            await _orderRepository.UpdateOrderMenu(_orderMenu);

            return Success(_orderMenu);
        }
    }
}
