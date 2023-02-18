using BusTour.AppServices.TourProcess.Steps;
using BusTour.Data.Repositories.Tours;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Infrastructure.Process;
using System.Threading.Tasks;

namespace BusTour.AppServices.TourProcess
{
    [InjectAsScoped]
    public class TourProcess : ProcessBase<Tour, TourProcessState>, ITourProcess
    {
        public TourProcess(TourProcessRepository tourProcessRepository, IProcessAudit audit)
            : base(tourProcessRepository, audit)
        {
        }

        public override string StartStepName => nameof(TourDraftStep);

        protected override int? ObjectId => Context?.Id;

        protected override bool IsUsing(Tour use, int id)
        {
            return use?.Id == id;
        }

        protected override Task<Tour> LoadAsync(int id)
        {
            return IoC.GetRequiredService<ITourRepository>()
                .GetAsync(id);
        }
    }
}
