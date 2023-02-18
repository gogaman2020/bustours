using BusTour.AppServices.TourProcess.Steps;
using System.Threading.Tasks;
using Infrastructure.Process.Commands;
using Infrastructure.Process.Args;

namespace BusTour.AppServices.TourProcess.Commands
{
    public sealed class CommandCancel : TourCommandBase
    {
        public CommandCancel(TourStepBase step)
            : base(step)
        {
        }

        public override string Name => TourStepCommand.Cancel;

        public override ValueTask<StepCommandResult> ExecuteAsync(StepCommandArgs commandArgs)
        {
            return Result(nameof(TourCanceledStep), commandArgs);
        }
    }
}
