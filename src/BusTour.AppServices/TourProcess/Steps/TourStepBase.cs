using BusTour.Domain.Entities;
using Infrastructure.Process;

namespace BusTour.AppServices.TourProcess.Steps
{
    public abstract class TourStepBase : StepBase<Tour>
    {
        public TourStepBase(TourProcess process, string nextStepName = null, string returnStepName = null, string cancelStepName = null)
            : base(process, null, nextStepName, returnStepName, cancelStepName)
        {
        }
    }
}
