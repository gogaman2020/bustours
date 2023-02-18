using Infrastructure.Process.Args;
using System.Threading.Tasks;

namespace Infrastructure.Process.Commands
{
    /// <summary>
    /// Интерфейс команды процесса
    /// </summary>
    public interface IStepCommand
    {
        /// <summary>
        /// Проверяет на валидность
        /// </summary>
        /// <returns></returns>
        ValueTask<ValidationResut> IsValidAsync();

        /// <summary>
        /// Проверяет на доступность
        /// </summary>
        /// <returns></returns>
        ValueTask<bool> IsAllowedAsync();

        /// <summary>
        /// Выполняет команду
        /// </summary>
        /// <param name="commandArgs">Параметры</param>
        /// <returns>Результат</returns>
        ValueTask<StepCommandResult> ExecuteAsync(StepCommandArgs commandArgs);
    }
}
