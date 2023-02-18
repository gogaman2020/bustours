using BusTour.AppServices.TourProcess.Steps;
using BusTour.Domain.Entities;
using Infrastructure.Process.Commands;

namespace BusTour.AppServices.TourProcess.Commands
{
    public abstract class TourCommandBase : StepCommandBase<Tour>
    {
        protected TourCommandBase(TourStepBase step)
            : base(step)
        {
        }

        protected override string StepToGo => null;
    }
}
