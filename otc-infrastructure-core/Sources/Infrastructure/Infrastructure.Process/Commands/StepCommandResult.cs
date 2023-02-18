namespace Infrastructure.Process.Commands
{
    /// <summary>
    /// Результат обработки шага
    /// </summary>
    public sealed class StepCommandResult
    {
        public StepCommandResult(string stepName, object args = null)
        {
            StepName = stepName;
            Args = args;
        }

        /// <summary>
        /// Название следующего шага
        /// </summary>
        public string StepName { get; private set; }

        /// <summary>
        /// Параметры
        /// </summary>
        public object Args { get; private set; }
    }
}
