using BusTour.AppServices.TourOrderProcess.Steps;
using Infrastructure.Process.Args;
using Infrastructure.Process.Commands;
using System.Threading.Tasks;

namespace BusTour.AppServices.TourOrderProcess.Commands
{
    public sealed class CommandWaitingForPayment : TourOrderCommandBase
    {
        public CommandWaitingForPayment(TourOrderStepBase step)
            : base(step)
        {
        }

        public override string Name => TourOrderStepCommand.WaitingForPaiment;

        public override ValueTask<StepCommandResult> ExecuteAsync(StepCommandArgs commandArgs)
        {
            return Result(nameof(TourOrderWaitingForPaymentStep), commandArgs);
        }
    }
}
