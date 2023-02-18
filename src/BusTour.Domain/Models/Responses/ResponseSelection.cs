using System.Collections.Generic;

namespace BusTour.Domain.Models.Responses
{
    /// <summary>
    /// Ответ на запрос сведений о статусах мест.
    /// </summary>
    public class ResponseSelection : BaseResponse
    {
        /// <summary>
        /// Выбор пользователя.
        /// </summary>
        public SelectionInfo Selection { get; set; }

        /// <summary>
        /// Коллекция столов.
        /// </summary>
        public List<ResponseTable> Tables { get; set; }

        /// <summary>
        /// Информация о заказе для выбранных пользователем мест.
        /// </summary>
        public ResponseSelectionOrderInfo OrderInfo { get; set; }
    }
}
