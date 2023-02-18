using Infrastructure.Process.Args;

namespace BusTour.AppServices.TourProcess.Args
{
    public class CreateStepCommandArgs: StepCommandArgs
    {
        public CreateStepCommandArgs(string name)
            : base(name)
        {
        }

        public bool IsService { get; set; }
        public bool IsPaid { get; set; }
        public bool IsDelete { get; set; }
    }
}
