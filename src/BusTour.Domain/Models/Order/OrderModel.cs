using BusTour.Domain.Enums;
using BusTour.Domain.Entities;
using System.Collections.Generic;

namespace BusTour.Domain.Models.Order
{
    public class OrderModel
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ИД тура.
        /// </summary>
        public int TourId { get; set; }

        /// <summary>
        /// Итоговая сумма заказа
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Тип заказа
        /// </summary>
        public OrderType Type { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Специальное меню
        /// </summary>
        public string SpecialRequests { get; set; }

        /// <summary>
        /// Промокод ИД
        /// </summary>
        public int? PromoCodeId { get; set; }

        /// <summary>
        /// Сертификат ИД
        /// </summary>
        public int? CertificateId { get; set; }

        /// <summary>
        /// Конфликты одобрены
        /// </summary>
        public bool IsConflictsApproved { get; set; }

        /// <summary>
        /// Признак группового заказа
        /// </summary>
        public bool IsGroup { get; set; }

        public SelectionVariant SeatingType { get; set; }

        /// <summary>
        /// Количество гостей
        /// </summary>
        public int GuestCount { get; set; }

        /// <summary>
        /// Количество гостей в инвалидностью
        /// </summary>
        public int DisabledGuestCount { get; set; }

        public OrderClientModel Client { get; set; }

        public OrderPrivateHireModel PrivateHire { get; set; }

        public List<OrderTable> Tables { get; set; }

        public List<OrderSeatModel> Seats { get; set; }

        public List<OrderMenuModel> Menus { get; set; }

        public List<OrderBeverageModel> Beverages { get; set; }

        public List<OrderSurpriseModel> Surpises { get; set; }

        public OrderModel()
        {
            Tables = new List<OrderTable>();
            Seats = new List<OrderSeatModel>();
            Menus = new List<OrderMenuModel>();
            Beverages = new List<OrderBeverageModel>();
            Surpises = new List<OrderSurpriseModel>();
        }

        //public OrderModel(Entities.Order order) : this()
        //{
        //    Id = order.Id;
        //    Id = order.Seats;
        //    Id = order.Id;
        //    Id = order.Id;
        //}
    }
}
