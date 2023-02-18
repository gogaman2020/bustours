using Infrastructure.Process.Commands;

namespace Infrastructure.Process
{
    /// <summary>
    /// Результат выполнения команды
    /// </summary>
    public sealed class StepResult
    {
        public StepResult(StepCommandAction action, StepCommandResult result)
        {
            Action = action;
            Result = result;
        }

        /// <summary>
        /// Действие
        /// </summary>
        public StepCommandAction Action { get; private set; }

        /// <summary>
        /// Результат шага
        /// </summary>
        public StepCommandResult Result { get; private set; }
    }
}
