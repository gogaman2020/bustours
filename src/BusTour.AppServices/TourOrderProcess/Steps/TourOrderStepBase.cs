using BusTour.Domain.Entities;
using Infrastructure.Process;

namespace BusTour.AppServices.TourOrderProcess.Steps
{
    public abstract class TourOrderStepBase : StepBase<Order>
    {
        public TourOrderStepBase(TourOrderProcess process, string nextStepName = null, string returnStepName = null, string cancelStepName = null)
            : base(process, null, nextStepName, returnStepName, cancelStepName)
        {
        }
    }
}
