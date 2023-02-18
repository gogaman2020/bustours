using Infrastructure.Db.Common;

namespace BusTour.Domain.Entities
{
    /// <summary>
    /// Место для выбора.
    /// </summary>
    public class SelectionSeat : BaseEntity
    {
        /// <summary>
        /// Наименование.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Признак, что место недоступно для выбора.
        /// </summary>
        public bool? IsLocked { get; set; }

        /// <summary>
        /// Id заказа.
        /// </summary>
        public int? OrderId { get; set; }

        /// <summary>
        /// Тип сиденья.
        /// </summary>
        public SeatType Type { get; set; }
    }
}
