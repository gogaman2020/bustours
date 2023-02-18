using BusTour.AppServices.TourProcess.Commands;
using BusTour.Domain.Attributes;
using BusTour.Domain.Enums;
using Infrastructure.Process.Commands;
using System.Collections.Generic;

namespace BusTour.AppServices.TourProcess.Steps
{
    [StepEnumItemRelation(TourState.Draft)]
    public sealed class TourDraftStep : TourStepBase
    {
        public TourDraftStep(TourProcess process)
            : base(process)
        {
        }

        protected override IEnumerable<StepCommandDescriptor> FillCommandDescriptors() => new[]
        {
            new StepCommandDescriptor(new CommandActive(this), "Save"),
            new StepCommandDescriptor(new CommandDelete(this), "Delete")
        };
    }
}
