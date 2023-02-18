namespace Infrastructure.Process.Commands
{
    /// <summary>
    /// Команда Cancel
    /// </summary>
    /// <typeparam name="TP"></typeparam>
    /// <typeparam name="TS"></typeparam>
    /// <typeparam name="TO"></typeparam>
    public class StepCommandCancel<T> : StepCommandBase<T>
        where T : IProcessStep<T>
    {
        public StepCommandCancel(T step) : base(step)
        {
        }

        protected override string StepToGo => Step.CancelStepName;

        public override string Name => StepCommands.Cancel;
    }
}
