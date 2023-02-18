using BusTour.Domain.Enums;
using System;

namespace BusTour.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class StepEnumItemRelationAttribute : Attribute
    {
        public Enum State { get; }

        public StepEnumItemRelationAttribute(TourState state)
        {
            State = state;
        }

        public StepEnumItemRelationAttribute(OrderState state)
        {
            State = state;
        }
    }
}
