using BusTour.Data.Repositories.Tours;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System.Threading.Tasks;

namespace BusTour.AppServices.TourService.Queries
{
    [InjectAsScoped]
    public class GetTourQuery : MediatorQuery<Tour>
    {
        public override async Task<MediatorCommandResult<Tour>> ExecuteAsync()
        {
            if (!int.TryParse(Id, out int id))
            {
                return Fail("Type mismatch");
            }

            return Success(await IoC.GetRequiredService<ITourRepository>().GetTourAsync(id));
        }
    }
}