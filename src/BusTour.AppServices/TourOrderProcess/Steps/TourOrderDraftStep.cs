using BusTour.AppServices.TourOrderProcess.Commands;
using BusTour.Domain.Attributes;
using BusTour.Domain.Enums;
using Infrastructure.Process.Commands;
using System.Collections.Generic;

namespace BusTour.AppServices.TourOrderProcess.Steps
{
    [StepEnumItemRelation(OrderState.Draft)]
    public sealed class TourOrderDraftStep : TourOrderStepBase
    {
        public TourOrderDraftStep(TourOrderProcess process)
            : base(process)
        {
        }

        protected override IEnumerable<StepCommandDescriptor> FillCommandDescriptors() => new[]
        {
            new StepCommandDescriptor(new CommandWaitingForPayment(this), "Save"),
            new StepCommandDescriptor(new CommandCancel(this), "Cancel")
        };
    }
}
