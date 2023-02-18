using BusTour.AppServices.TourProcess.Commands;
using BusTour.Domain.Attributes;
using BusTour.Domain.Enums;
using Infrastructure.Process.Commands;
using System.Collections.Generic;

namespace BusTour.AppServices.TourProcess.Steps
{
    [StepEnumItemRelation(TourState.Active)]
    public sealed class TourActiveStep : TourStepBase
    {
        public TourActiveStep(TourProcess process)
            : base(process)
        {
        }

        protected override IEnumerable<StepCommandDescriptor> FillCommandDescriptors() => new[]
        {
            new StepCommandDescriptor(new CommandCancelRequest(this), "Cancel request"),
            new StepCommandDescriptor(new CommandCancel(this), "Cancel"),
            new StepCommandDescriptor(new CommandDelete(this), "Delete")
        };
    }
}
