using BusTour.Data.Repositories.Tours;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusTour.AppServices.TourService.Queries
{
    [InjectAsScoped]
    public class GetCitiesQuery : MediatorQuery<List<City>>
    {
        public override async Task<MediatorCommandResult<List<City>>> ExecuteAsync()
        {
            return Success(await IoC.GetRequiredService<ITourRepository>().GetCitiesAsync());
        }
    }
}