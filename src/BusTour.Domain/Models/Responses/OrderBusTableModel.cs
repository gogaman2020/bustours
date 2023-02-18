using System.Collections.Generic;
using System.Linq;
using BusTour.Domain.Entities;
using Newtonsoft.Json;

namespace BusTour.Domain.Models.Responses
{
    /// <summary>
    /// Информация о столе.
    /// </summary>
    public class OrderBusTableModel
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
        /// Выбран в текщем заказе.
        /// </summary>
        public bool IsSelected => Seats.All(x => x.IsSelected);

        /// <summary>
        /// Свободен.
        /// </summary>
        [JsonIgnore]
        public bool IsFree => Seats.All(x => x.IsFree);

        /// <summary>
        /// ВИП.
        /// </summary>
        [JsonIgnore]
        public bool IsVip => Table.IsVip;

        /// <summary>
        /// Обычный.
        /// </summary>
        [JsonIgnore]
        public bool IsRegular => !IsVip;

        /// <summary>
        /// Доступен для выбора.
        /// </summary>
        [JsonIgnore]
        public bool AllowedByRules { get; set; }

        /// <summary>
        /// Стол.
        /// </summary>
        [JsonIgnore]
        public Table Table { get; set; }

        /// <summary>
        /// Сущность заказа.
        /// </summary>
        [JsonIgnore]
        public Entities.Order Order { get; set; }

        /// <summary>
        /// Может быть выбран.
        /// </summary>
        public bool IsAvailable => AllowedByRules || IsSelected;

        /// <summary>
        /// Может быть апгрейнутым до стола.
        /// </summary>
        public bool IsAvailableForUpgrade { get; set; } = false;

        /// <summary>
        /// Коллекция мест за столом.
        /// </summary>
        public List<OrderBusSeatModel> Seats { get; set; }
    }
}
