using Infrastructure.Process.Args;
using Infrastructure.Process.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Process
{
    /// <summary>
    /// Шаг процесса
    /// </summary>
    public interface IProcessStep<T>
    {
        /// <summary>
        /// Шаг при отмене текущего
        /// </summary>
        string CancelStepName { get; }

        /// <summary>
        /// Описание команд
        /// </summary>
        IEnumerable<StepCommandDescriptor> CommandDescriptors { get; }

        /// <summary>
        /// Название
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Название следующего шага
        /// </summary>
        string NextStepName { get; }

        /// <summary>
        /// Процесс
        /// </summary>
        IProcess<T> Process { get; }

        /// <summary>
        /// Название шага при возврате
        /// </summary>
        string ReturnStepName { get; }

        /// <summary>
        /// Тип объекта состояния
        /// </summary>
        Type StateType { get; }

        /// <summary>
        /// Выполняет команду
        /// </summary>
        /// <param name="commandArgs">Параметры</param>
        /// <returns>Результат выполнения</returns>
        ValueTask<StepResult> CommandAsync(StepCommandArgs commandArgs);

        /// <summary>
        /// Получает объект состояния
        /// </summary>
        /// <returns></returns>
        object GetState();

        /// <summary>
        /// Устанавливает объект состояния
        /// </summary>
        /// <param name="state">Объект состояния</param>
        void SetState(object state);
    }
}