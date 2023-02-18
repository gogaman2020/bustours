using BusTour.Data.Repositories.Orders;
using BusTour.Data.Repositories.Tours;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System.Threading.Tasks;

namespace BusTour.AppServices.OrderService
{
    [InjectAsScoped]
    public class GetOrderCommand : HighLevelMediatorCommand<Order>
    {
        private readonly IOrderRepository _tourOrderRepository;
        private readonly ITourRepository _tourRepository;

        public GetOrderCommand()
        {
            _tourOrderRepository = IoC.GetRequiredService<IOrderRepository>();
            _tourRepository = IoC.GetRequiredService<ITourRepository>();
        }

        public override async Task<MediatorCommandResult<Order>> ExecuteAsync()
        {
            if (!int.TryParse(Id, out int id))
            {
                return Fail("Type mismatch");
            }

            var order = await _tourOrderRepository.GetAsync(id);
            if (order == null)
            {
                return Fail();
            }

            order.Tour = await _tourRepository.GetAsync(order.TourId, fillNested: true);

            return Success(order);
        }
    }
}
