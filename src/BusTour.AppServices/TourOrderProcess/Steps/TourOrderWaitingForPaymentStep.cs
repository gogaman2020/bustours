using BusTour.AppServices.TourOrderProcess.Commands;
using BusTour.Domain.Attributes;
using BusTour.Domain.Enums;
using Infrastructure.Process.Commands;
using System.Collections.Generic;

namespace BusTour.AppServices.TourOrderProcess.Steps
{
    [StepEnumItemRelation(OrderState.WaitingForPayment)]
    public sealed class TourOrderWaitingForPaymentStep : TourOrderStepBase
    {
        public TourOrderWaitingForPaymentStep(TourOrderProcess process)
            : base(process)
        {
        }

        protected override IEnumerable<StepCommandDescriptor> FillCommandDescriptors() => new[]
        {
            new StepCommandDescriptor(new CommandPayment(this), "Paid"),
            new StepCommandDescriptor(new CommandCancel(this), "Cancel"),
        };
    }
}
