using System;
using System.Threading.Tasks;

namespace Infrastructure.Process.Repositories
{
    /// <summary>
    /// Интрефейс репозитария шагов
    /// </summary>
    public interface IProcessStepRepository
    {
        /// <summary>
        /// ИД процесса
        /// </summary>
        int ProcessId { get; }

        /// <summary>
        /// Текущий шаг
        /// </summary>
        string CurrentStepName { get; }

        /// <summary>
        /// Устанавливает текущий шаг
        /// </summary>
        /// <param name="currentStep"></param>
        /// <returns></returns>
        Task UpdateCurrentStepNameAsync(string currentStep);

        /// <summary>
        /// Получает состояние из типа
        /// </summary>
        /// <param name="objectType">Тип</param>
        /// <returns>Объект</returns>
        Task<object> GetCurrentStepDataAsync(Type objectType);

        /// <summary>
        /// Сохраняет состояние
        /// </summary>
        /// <param name="data">Объект</param>
        /// <returns></returns>
        Task SaveStepDataAsync(object data);
    }
}
