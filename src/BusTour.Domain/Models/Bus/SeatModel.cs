using BusTour.Domain.Entities;

namespace BusTour.Domain.Models.Bus
{
    /// <summary>
    /// Модель места.
    /// </summary>
    public class SeatModel
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Номер.
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Признак, что место недоступно для выбора.
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Признак, что место выбрано.
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Признак, что место доступно для выбора.
        /// </summary>
        public bool IsAvailable => !IsLocked && !IsSelected;

        /// <summary>
        /// Тип сиденья.
        /// </summary>
        public SeatType Type { get; set; }
    }
}
