using Infrastructure.Process.Args;
using System.Threading.Tasks;

namespace Infrastructure.Process.Commands
{
    /// <summary>
    /// Базовая команды
    /// </summary>
    public abstract class StepCommandBase : IStepCommand
    {
        /// <summary>
        /// Название
        /// </summary>
        public abstract string Name { get; }

        public virtual ValueTask<ValidationResut> IsValidAsync()
        {
            return new ValueTask<ValidationResut>(ValidationResut.Valid());
        }

        public virtual ValueTask<bool> IsAllowedAsync()
        {
            return new ValueTask<bool>(true);
        }

        public abstract ValueTask<StepCommandResult> ExecuteAsync(StepCommandArgs commandArgs);

        /// <summary>
        /// Создает результат из аргументов
        /// </summary>
        /// <param name="stepName">Шаг</param>
        /// <param name="args">Параметры</param>
        /// <returns></returns>
        protected static ValueTask<StepCommandResult> Result(string stepName = null, object args = null)
        {
            return new ValueTask<StepCommandResult>(new StepCommandResult(stepName, args));
        }
    }

    /// <summary>
    /// Типизированная базовая команда
    /// </summary>
    /// <typeparam name="TP">Тип процесса</typeparam>
    /// <typeparam name="TS">Тип шага</typeparam>
    /// <typeparam name="TO">Тип объекта процесса</typeparam>
    public abstract class StepCommandBase<T> : StepCommandBase
    {
        /// <summary>
        /// Шаг
        /// </summary>
        protected readonly IProcessStep<T> Step;

        protected StepCommandBase(IProcessStep<T> step)
        {
            Step = step;
        }

        /// <summary>
        /// Шаг для перехода
        /// </summary>
        protected abstract string StepToGo { get; }

        public override ValueTask<bool> IsAllowedAsync()
        {
            return new ValueTask<bool>(!string.IsNullOrEmpty(StepToGo));
        }

        public override ValueTask<StepCommandResult> ExecuteAsync(StepCommandArgs commandArgs)
        {
            return Result(StepToGo, null);
        }
    }
}
