using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using System.Collections.Generic;

namespace BusTour.Domain.Models.Tour
{
    /// <summary>
    /// Модель заказа тура
    /// </summary>
    public class TourOrderGridModel
    {
        /// <summary>
        /// Идентификатор заказа
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Сиденья
        /// </summary>
        public List<OrderSeat> Seats { get; set; }

        /// <summary>
        /// Имя клиента
        /// </summary>
        public Client Client { get; set; }

        /// <summary>
        /// Статус заказа
        /// </summary>
        public OrderState? OrderState { get; set; }

        /// <summary>
        /// Наличие конфликтов у заказа
        /// </summary>
        public bool Conflict { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Хеш заказа
        /// </summary>
        public string Hash { get; set; }
    }
}
