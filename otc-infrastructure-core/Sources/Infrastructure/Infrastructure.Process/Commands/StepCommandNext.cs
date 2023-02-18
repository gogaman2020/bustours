namespace Infrastructure.Process.Commands
{
    /// <summary>
    /// Команда Next
    /// </summary>
    public class StepCommandNext<T> : StepCommandBase<T>
        where T : IProcessStep<T>
    {
        public StepCommandNext(T step) : base(step)
        {
        }

        protected override string StepToGo => Step.NextStepName;
        public override string Name => StepCommands.Next;
    }
}
