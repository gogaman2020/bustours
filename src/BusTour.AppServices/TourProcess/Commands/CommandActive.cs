using BusTour.AppServices.TourProcess.Steps;
using BusTour.Common.Services;
using BusTour.Domain.Models.Auth;
using Infrastructure.Common.DI;
using Infrastructure.Process.Args;
using Infrastructure.Process.Commands;
using System.Threading.Tasks;

namespace BusTour.AppServices.TourProcess.Commands
{
    public sealed class CommandActive : TourCommandBase
    {
        public CommandActive(TourStepBase step)
            : base(step)
        {
        }

        public override string Name => TourStepCommand.Active;

        public override ValueTask<StepCommandResult> ExecuteAsync(StepCommandArgs commandArgs)
        {
            return Result(nameof(TourActiveStep), null);
        }
    }
}
