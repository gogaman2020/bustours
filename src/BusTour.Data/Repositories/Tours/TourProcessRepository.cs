using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Infrastructure.Process.Repositories;

namespace BusTour.Data.Repositories.Tours
{
    [InjectAsSingleton]
    public class TourProcessRepository : ProcessRepositoryBase<Tour, TourProcessState>
    {
        protected override int GetIdentifier(Tour state)
        {
            return state.Id;
        }
    }
}
