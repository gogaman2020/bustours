using BusTour.Data.Repositories.BusRepository;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusTour.AppServices.TourService.Queries
{
    [InjectAsScoped]
    public class GetBusesQuery : MediatorQuery<List<Bus>>
    {
        public override async Task<MediatorCommandResult<List<Bus>>> ExecuteAsync()
        {
            return Success(await IoC.GetRequiredService<IBusRepository>().GetBusesAsync());
        }
    }
}