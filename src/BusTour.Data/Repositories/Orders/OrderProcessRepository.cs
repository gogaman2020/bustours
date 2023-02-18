using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Infrastructure.Process.Repositories;

namespace BusTour.Data.Repositories.Orders
{
    [InjectAsSingleton]
    public class OrderProcessRepository : ProcessRepositoryBase<Order, OrderProcessState>
    {
        protected override int GetIdentifier(Order state)
        {
            return state.Id;
        }
    }
}
