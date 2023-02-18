namespace Infrastructure.Process.Commands
{
    /// <summary>
    /// Команда Return
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StepCommandReturn<T> : StepCommandBase<T>
        where T : IProcessStep<T>
    {
        public StepCommandReturn(T step) : base(step)
        {
        }

        protected override string StepToGo => Step.ReturnStepName;
        public override string Name => StepCommands.Return;
    }
}
