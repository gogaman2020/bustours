using BusTour.Domain.Models.Responses;
using BusTour.Domain.Models.Selection;
using System.Threading.Tasks;

namespace BusTour.AppServices.SelectionService
{
    /// <summary>
    /// Интерфейс сервиса подбора мест.
    /// </summary>
    public interface ISelectionService
    {
        /// <summary>
        /// Подбор мест.
        /// </summary>
        /// <param name="request">Параметры подбора.</param>
        /// <returns>Результат подбора мест.</returns>
        Task<ResponseSelection> SelectAsync(SelectionInfo request);

        /// <summary>
        /// Формирует модель автобуса для конкретного заказа с возможностью ручного выбора мест. 
        /// Учитывает занятые места другими заказами, а места текущего заказа доступны для выбора.
        /// </summary>
        /// <param name="info">Параметры модели автобуса</param>
        /// <returns>Редактируемую в ручном режиме модель автобуса</returns>
        Task<ResponseSelection> GetSelectableBusModelForOrderAsync(SelectionInfo info);
    }
}
