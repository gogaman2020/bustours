using BusTour.Domain.Entities;
using Infrastructure.Db.Repositories;
using Infrastructure.Process.Repositories;

namespace BusTour.Data.Repositories.Tours
{
    /// <summary>
    /// Репозитарий состояния претензии
    /// </summary>
    public interface ITourProcessRepository : IProcessRepository<Tour>, ICrudRepository<TourProcessState>
    {
    }
}
