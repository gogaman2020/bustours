using BusTour.Domain.Enums;
using BusTour.Domain.Models.Bus;
using System.Collections.Generic;

namespace BusTour.Domain.Models.Responses
{
    /// <summary>
    /// Информация о выборе пользователя.
    /// </summary>
    public class SelectionInfo
    { 
        /// <summary>
        /// ИД тура.
        /// </summary>
        public int TourId { get; set; }

        /// <summary>
        /// Количество мест.
        /// </summary>
        public byte GuestCount { get; set; }

        /// <summary>
        /// Количество мест для инвалидов
        /// </summary>
        public int? DisabledGuestCount { get; set; }

        /// <summary>
        /// Вариант выбора мест.
        /// </summary>
        public SelectionVariant SeatingType { get; set; }

        /// <summary>
        /// Коллекция выбранных объектов.
        /// </summary>
        public List<BusObject> SelectedObjects { get; set; }

        /// <summary>
        /// Объект, по которому кликнул пользователь.
        /// </summary>
        public BusObject ClickedObject { get; set; }

        /// <summary>
        /// Ручной выбор мест
        /// </summary>
        public bool ManualSelectionMode { get; set; }

        /// <summary>
        /// Тип заказа
        /// </summary>
        public OrderType OrderType { get; set; }

        public int? OrderId { get; set; }
    }
}
