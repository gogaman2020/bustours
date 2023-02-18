using Infrastructure.Process.Args;

namespace BusTour.AppServices.TourOrderProcess.Args
{
    public class PayStepCommandArgs: StepCommandArgs
    {
        public PayStepCommandArgs(string name)
            : base(name)
        {
        }

        public bool IsPaid { get; set; }
    }
}
