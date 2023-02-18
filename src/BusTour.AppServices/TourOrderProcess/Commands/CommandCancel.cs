using BusTour.AppServices.TourOrderProcess.Steps;
using Infrastructure.Process.Args;
using Infrastructure.Process.Commands;
using System.Threading.Tasks;

namespace BusTour.AppServices.TourOrderProcess.Commands
{
    public sealed class CommandCancel : TourOrderCommandBase
    {
        public CommandCancel(TourOrderStepBase step)
            : base(step)
        {
        }

        public override string Name => TourOrderStepCommand.Cancel;

        public override ValueTask<StepCommandResult> ExecuteAsync(StepCommandArgs commandArgs)
        {
            return Result(nameof(TourOrderCanceledStep), commandArgs);
        }
    }
}
