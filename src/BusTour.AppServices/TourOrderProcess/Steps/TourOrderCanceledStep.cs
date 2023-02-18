using BusTour.Domain.Attributes;
using BusTour.Domain.Enums;
using Infrastructure.Process.Commands;
using System;
using System.Collections.Generic;

namespace BusTour.AppServices.TourOrderProcess.Steps
{
    [StepEnumItemRelation(OrderState.Canceled)]
    public sealed class TourOrderCanceledStep : TourOrderStepBase
    {
        public TourOrderCanceledStep(TourOrderProcess process)
            : base(process)
        {
        }

        protected override IEnumerable<StepCommandDescriptor> FillCommandDescriptors() =>
            Array.Empty<StepCommandDescriptor>();
    }
}
