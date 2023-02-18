using Infrastructure.Common.Context;
using Infrastructure.Process.Args;
using Infrastructure.Process.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Process
{
    /// <summary>
    /// Интерфейс процесса
    /// </summary>
    public interface IProcess<T>: IObjectContext<T>
    {
        /// <summary>
        /// Текущий шаг
        /// </summary>
        string CurrentStepName { get; }

        /// <summary>
        /// Стартовый шаг при создании процесса
        /// </summary>
        string StartStepName { get; }

        /// <summary>
        /// Доспутные шаги
        /// </summary>
        IEnumerable<string> StepNames { get; }

        /// <summary>
        /// Установка текущего шага из БД
        /// </summary>
        /// <returns></returns>
        ValueTask<IProcessStep<T>> GetCurrentStepAsync();

        /// <summary>
        /// Инициализация
        /// </summary>
        /// <returns></returns>
        ValueTask InitStateAsync();

        /// <summary>
        /// Проверка доступности выполнения шага
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        ValueTask<bool> IsAllowedAsync(StepCommandDescriptor descriptor);

        /// <summary>
        /// Сброс процесса
        /// </summary>
        void Reset();

        /// <summary>
        /// Выполняет команду
        /// </summary>
        /// <param name="commandArgs">Параметры</param>
        /// <returns></returns>
        Task SendCommandAsync(StepCommandArgs commandArgs);

        /// <summary>
        /// Выполняет команду по имени
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task SendCommandAsync(string name);

        Task SendCommandAsync(int id, string name)
        {
            return SendCommandAsync(name);
        }
    }
}