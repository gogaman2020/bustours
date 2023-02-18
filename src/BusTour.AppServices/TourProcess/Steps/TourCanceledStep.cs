using BusTour.AppServices.TourProcess.Commands;
using BusTour.Domain.Attributes;
using BusTour.Domain.Enums;
using Infrastructure.Process.Commands;
using System.Collections.Generic;

namespace BusTour.AppServices.TourProcess.Steps
{
    [StepEnumItemRelation(TourState.Canceled)]
    public sealed class TourCanceledStep : TourStepBase
    {
        public TourCanceledStep(TourProcess process)
            : base(process)
        {
        }

        protected override IEnumerable<StepCommandDescriptor> FillCommandDescriptors() => new[]
        {
            new StepCommandDescriptor(new CommandActive(this), "Active"),
            new StepCommandDescriptor(new CommandDelete(this), "Delete")
        };
    }
}
