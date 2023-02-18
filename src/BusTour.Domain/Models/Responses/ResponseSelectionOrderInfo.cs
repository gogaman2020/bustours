using System.Collections.Generic;

namespace BusTour.Domain.Models.Responses
{
    /// <summary>
    /// Информация о выбранных местах для вкладки "Меню"
    /// </summary>
    public class ResponseSelectionOrderInfo
    {
        /// <summary>
        /// Столы.
        /// </summary>
        public List<ResponseSelectionOrderTableInfo> Tables { get; } = new List<ResponseSelectionOrderTableInfo>();

        /// <summary>
        /// Гости.
        /// </summary>
        public List<ResponseSelectionMenuGuestInfo> Guests { get; } = new List<ResponseSelectionMenuGuestInfo>();
    }
}
