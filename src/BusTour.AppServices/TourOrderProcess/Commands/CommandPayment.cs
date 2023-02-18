using BusTour.AppServices.TourOrderProcess.Args;
using BusTour.AppServices.TourOrderProcess.Steps;
using Infrastructure.Process.Args;
using Infrastructure.Process.Commands;
using System.Threading.Tasks;

namespace BusTour.AppServices.TourOrderProcess.Commands
{
    public sealed class CommandPayment : TourOrderCommandBase
    {
        public CommandPayment(TourOrderStepBase step)
            : base(step)
        {
        }

        public override string Name => TourOrderStepCommand.Payment;

        public override ValueTask<StepCommandResult> ExecuteAsync(StepCommandArgs commandArgs)
        {
            var create = commandArgs as PayStepCommandArgs;
            if (commandArgs != null)
            {
                if (create.IsPaid)
                {
                    return Result(nameof(TourOrderPaidStep), commandArgs);
                }
                else
                {
                    return Result(nameof(TourOrderNotPaidStep), commandArgs);
                }
            }
            return Result();
        }
    }
}
