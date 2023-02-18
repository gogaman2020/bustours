using BusTour.Domain.Attributes;
using BusTour.Domain.Enums;
using Infrastructure.Process.Commands;
using System;
using System.Collections.Generic;

namespace BusTour.AppServices.TourOrderProcess.Steps
{
    [StepEnumItemRelation(OrderState.NotPaid)]
    public sealed class TourOrderNotPaidStep : TourOrderStepBase
    {
        public TourOrderNotPaidStep(TourOrderProcess process)
            : base(process)
        {
        }

        protected override IEnumerable<StepCommandDescriptor> FillCommandDescriptors() =>
            Array.Empty<StepCommandDescriptor>();
    }
}
