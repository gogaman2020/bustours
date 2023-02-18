using System.Threading.Tasks;
using BusTour.AppServices.TourProcess.Steps;
using Infrastructure.Process;
using Infrastructure.Process.Args;
using Infrastructure.Process.Commands;

namespace BusTour.AppServices.TourProcess.Commands
{
    public sealed class CommandDelete : TourCommandBase
    {
        public CommandDelete(TourStepBase step)
            : base(step)
        {
        }

        public override string Name => TourStepCommand.Delete;

        public override ValueTask<StepCommandResult> ExecuteAsync(StepCommandArgs commandArgs)
        {
            return Result(nameof(TourDeletedStep), commandArgs);
        }
    }
}
