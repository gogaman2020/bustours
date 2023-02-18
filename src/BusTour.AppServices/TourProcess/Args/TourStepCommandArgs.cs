using Infrastructure.Process.Args;

namespace BusTour.AppServices.TourProcess.Args
{
    public class TourStepCommandArgs : StepCommandArgs
    {
        public TourStepCommandArgs(string name)
            : base(name)
        {
        }

        public bool CancelRequest { get; set; }
    }
}
