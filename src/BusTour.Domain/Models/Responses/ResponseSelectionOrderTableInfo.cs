using System.Collections.Generic;

namespace BusTour.Domain.Models.Responses
{
    /// <summary>
    /// Информация о выбранном столе для вкладки "Меню".
    /// </summary>
    public class ResponseSelectionOrderTableInfo
    { 
        /// <summary>
        /// ИД стола.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Положение стола по оси X.
        /// </summary>
        public short X { get; set; }

        /// <summary>
        /// Положение стола по оси Y.
        /// </summary>
        public short Y { get; set; }

        /// <summary>
        /// Места.
        /// </summary>
        public List<ResponseSelectionOrderSeatInfo> Seats { get; } = new List<ResponseSelectionOrderSeatInfo>();
    }
}
