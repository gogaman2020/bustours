using BusTour.AppServices.TourProcess.Steps;
using Infrastructure.Process.Args;
using Infrastructure.Process.Commands;
using System.Threading.Tasks;

namespace BusTour.AppServices.TourProcess.Commands
{
    public sealed class CommandCancelRequest : TourCommandBase
    {
        public CommandCancelRequest(TourStepBase step)
            : base(step)
        {
        }

        public override string Name => TourStepCommand.CancelRequest;

        public override ValueTask<StepCommandResult> ExecuteAsync(StepCommandArgs commandArgs)
        {
            return Result(nameof(TourCancelRequestStep), commandArgs);
        }
    }
}
