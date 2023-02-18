using BusTour.Domain.Entities;
using Infrastructure.Process;
using Infrastructure.Process.Commands;

namespace BusTour.AppServices.TourOrderProcess.Commands
{
    public abstract class TourOrderCommandBase : StepCommandBase<Order>
    {
        protected TourOrderCommandBase(IProcessStep<Order> step) : base(step)
        {
        }

        protected override string StepToGo => null;
    }
}
