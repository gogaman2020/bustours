namespace Infrastructure.Process.Args
{
    /// <summary>
    /// Параметры команды
    /// </summary>
    public class StepCommandArgs
    {
        public StepCommandArgs(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Название шага
        /// </summary>
        public string Name { get; }
    }
}
