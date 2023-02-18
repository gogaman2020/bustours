using BusTour.Domain.Entities;
using Infrastructure.Db.Repositories;
using Infrastructure.Process.Repositories;

namespace BusTour.Data.Repositories.Orders
{
    /// <summary>
    /// Репозитарий состояния претензии
    /// </summary>
    public interface IOrderProcessRepository: IProcessRepository<Order>, ICrudRepository<OrderProcessState>
    {
    }
}
