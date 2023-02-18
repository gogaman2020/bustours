using BusTour.AppServices.TourProcess.Commands;
using BusTour.Domain.Attributes;
using BusTour.Domain.Enums;
using Infrastructure.Process.Commands;
using System.Collections.Generic;

namespace BusTour.AppServices.TourProcess.Steps
{
    [StepEnumItemRelation(TourState.CancelRequest)]
    public sealed class TourCancelRequestStep : TourStepBase
    {
        public TourCancelRequestStep(TourProcess process)
            : base(process)
        {
        }

        protected override IEnumerable<StepCommandDescriptor> FillCommandDescriptors() => new[]
        {
            new StepCommandDescriptor(new CommandCancel(this), "Cancel"),
            new StepCommandDescriptor(new CommandDelete(this), "Delete")
        };
    }
}
