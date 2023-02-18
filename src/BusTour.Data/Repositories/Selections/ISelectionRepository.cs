using BusTour.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusTour.Data.Repositories.Selections
{
    /// <summary>
    /// Интерфейс репозитория для запросов алгоритма подбора мест.
    /// </summary>
    public interface ISelectionRepository
    {
        /// <summary>
        /// Получить информацию по состоянию автобуса.
        /// </summary>
        /// <param name="tourId">Идентификатор тура.</param>
        /// <returns>Информация по состоянию автобуса.</returns>
        Task<List<SelectionTable>> GetTourInfoAsync(int tourId);

        Task<List<ObjectPosition>> GetBusObjectsPositionsAsync(int tourId);
    }
}
