using BusTour.Data.Repositories.Orders;
using BusTour.Domain.Entities;
using BusTour.Domain.Models.Order;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System.Threading.Tasks;

namespace BusTour.AppServices.OrderService
{
    [InjectAsScoped]
    public class GetOrderExtrasCommand : HighLevelMediatorCommand<OrderExtras>
    {
        private readonly IOrderRepository _tourOrderProcessRepository;
        public GetOrderExtrasCommand()
        {
            _tourOrderProcessRepository = IoC.GetRequiredService<IOrderRepository>();
        }

        public override async Task<MediatorCommandResult<OrderExtras>> ExecuteAsync()
        {
            if (!int.TryParse(Id, out int id))
            {
                return Fail("Type mismatch");
            }

            var extras = await _tourOrderProcessRepository.GetExtrasAsync(id);
            if (extras == null)
            {
                return Fail();
            }

            return Success(extras);
        }
    }
}
