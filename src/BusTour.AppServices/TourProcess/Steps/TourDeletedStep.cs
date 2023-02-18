using BusTour.Domain.Attributes;
using BusTour.Domain.Enums;
using Infrastructure.Process.Commands;
using System;
using System.Collections.Generic;

namespace BusTour.AppServices.TourProcess.Steps
{
    [StepEnumItemRelation(TourState.Deleted)]
    public sealed class TourDeletedStep : TourStepBase
    {
        public TourDeletedStep(TourProcess process)
            : base(process)
        {
        }

        protected override IEnumerable<StepCommandDescriptor> FillCommandDescriptors() =>
            Array.Empty<StepCommandDescriptor>();
    }
}
