using Infrastructure.Process.Commands;

namespace Infrastructure.Process.Args
{
    /// <summary>
    /// Парметры команды Enter
    /// </summary>
    public class StepCommandArgsEnter : StepCommandArgs
    {
        public StepCommandArgsEnter(object args, string prevStepName)
            : base(StepCommands.Enter)
        {
            Args = args;
            PrevStepName = prevStepName;
        }

        /// <summary>
        /// Параметры команды
        /// </summary>
        public object Args { get; }

        /// <summary>
        /// Предыдущий шаг
        /// </summary>
        public string PrevStepName { get; }
    }
}
