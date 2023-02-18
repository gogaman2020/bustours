using BusTour.Domain.Entities;

namespace BusTour.Domain.Models.Responses
{
    /// <summary>
    /// Информация о месте за столом.
    /// </summary>
    public class ResponseSeat
    {
        /// <summary>
        /// ИД места.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Номер.
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Признак - место заблокировано.
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Признак - место выбрано.
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Признак - место доступно для выбора.
        /// </summary>
        public bool IsAvailable { get; set; }


        /// <summary>
        /// Id заказа
        /// </summary>
        public int? OrderId { get; set; }

        /// <summary>
        /// Тип сиденья.
        /// </summary>
        public SeatType Type { get; set; }
    }
}
