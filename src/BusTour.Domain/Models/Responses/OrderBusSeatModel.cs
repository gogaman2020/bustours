using BusTour.Domain.Entities;
using Newtonsoft.Json;

namespace BusTour.Domain.Models.Responses
{
    /// <summary>
    /// Информация о месте.
    /// </summary>
    public class OrderBusSeatModel
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Начальная цена.
        /// </summary>
        public decimal InitialPrice { get; set; }

        /// <summary>
        /// Цена
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Выбран в текущем заказе.
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Выбран в другом заказе.
        /// </summary>
        public bool IsOtherOrdered { get; set; }

        /// <summary>
        /// Доступен для выбора.
        /// </summary>
        [JsonIgnore]
        public bool IsAllowedByRules { get; set; }

        /// <summary>
        /// Не занят.
        /// </summary>
        [JsonIgnore]
        public bool IsFree => !IsOtherOrdered && !IsSelected;

        /// <summary>
        /// VIP место.
        /// </summary>
        [JsonIgnore]
        public bool IsVip => TableModel.Table.IsVip;

        /// <summary>
        /// Обычное место.
        /// </summary>
        [JsonIgnore]
        public bool IsRegular => !TableModel.Table.IsVip;

        /// <summary>
        /// Модель стола.
        /// </summary>
        [JsonIgnore]
        public OrderBusTableModel TableModel { get; set; }

        /// <summary>
        /// Сущность сидушки.
        /// </summary>
        [JsonIgnore]
        public Seat Seat { get; set; }

        /// <summary>
        /// Сущность заказа.
        /// </summary>
        [JsonIgnore]
        public Entities.Order Order { get; set; }

        /// <summary>
        /// Может быть выбран.
        /// </summary>
        public bool IsAvailable => !(IsOtherOrdered && !Order.IsGroup) && IsAllowedByRules;
    }
}
