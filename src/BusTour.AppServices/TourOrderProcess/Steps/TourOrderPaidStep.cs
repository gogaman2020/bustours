using BusTour.AppServices.TourOrderProcess.Commands;
using BusTour.Domain.Attributes;
using BusTour.Domain.Enums;
using Infrastructure.Process.Commands;
using System.Collections.Generic;

namespace BusTour.AppServices.TourOrderProcess.Steps
{
    [StepEnumItemRelation(OrderState.Paid)]
    public sealed class TourOrderPaidStep : TourOrderStepBase
    {
        public TourOrderPaidStep(TourOrderProcess process)
            : base(process)
        {
        }

        protected override IEnumerable<StepCommandDescriptor> FillCommandDescriptors() => new[]
        {
            new StepCommandDescriptor(new CommandCancel(this), "Cancel")
        };
    }
}
