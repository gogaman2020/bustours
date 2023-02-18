using BusTour.Domain.Enums;
using BusTour.Domain.Helpers;
using Infrastructure.Db.Common;
using System;
using System.Collections.Generic;

namespace BusTour.Domain.Entities
{
    /// <summary>
    /// Заказ
    /// </summary>
    public class Order : BaseEntity
    {
        /// <summary>
        /// Хэш заказа
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// ИД тура.
        /// </summary>
        public int TourId { get; set; }

        /// <summary>
        /// Дата создания заказа.
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Дата оплаты заказа.
        /// </summary>
        public DateTime? PaymentDate { get; set; }

        /// <summary>
        /// Тип стола
        /// </summary>
        public SelectionVariant TableType { get; set; }

        /// <summary>
        /// Количество маломобильных гостей
        /// </summary>
        public int DisabledGuestCount { get; set; }

        /// <summary>
        /// ИД клиента
        /// </summary>
        public int? ClientId { get; set; }

        /// <summary>
        /// ИД купона.
        /// </summary>
        public int? PromoCodeId { get; set; }

        /// <summary>
        /// ИД подарочного сертификата
        /// </summary>
        public int? CertificateId { get; set; }

        /// <summary>
        /// Скидка
        /// </summary>
        public decimal? Discount { get; set; }

        /// <summary>
        /// Итоговая сумма заказа
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Признак группового заказа
        /// </summary>
        public bool IsGroup { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Специальное меню
        /// </summary>
        public string SpecialRequests { get; set; }

        /// <summary>
        /// Количество гостей.
        /// </summary>
        public int GuestCount { get; set; }

        /// <summary>
        /// Информация о туре.
        /// </summary>
        [IgnoreField]
        public Tour Tour { get; set; }

        /// <summary>
        /// Информация о клиенте.
        /// </summary>
        public Client Client { get; set; }

        /// <summary>
        /// Оплата
        /// </summary>
        [IgnoreField]
        public Payment Payment { get; set; }

        /// <summary>
        /// Коллекция мест в заказе.
        /// </summary>
        [IgnoreField]
        public List<OrderSeat> Seats { get; set; }

        /// <summary>
        /// Коллекция меню в заказе.
        /// </summary>
        [IgnoreField]
        public List<OrderMenu> Menus { get; set; }

        /// <summary>
        /// Коллекция напитков в заказе.
        /// </summary>
        [IgnoreField]
        public List<OrderBeverage> Beverages { get; set; }

        /// <summary>
        /// Коллекция сюрпризов
        /// </summary>
        [IgnoreField]
        public List<OrderSurprise> Surprises { get; set; }

        /// <summary>
        /// Промокод
        /// </summary>
        public PromoCode PromoCode { get; set; }

        /// <summary>
        /// Подарочный сертификат
        /// </summary>
        public GiftCertificate GiftCertificate { get; set; }

        /// <summary>
        /// Тип заказа
        /// </summary>
        public OrderType Type
        {
            get
            {
                if (Tour != null && Tour.Type == TourType.Service)
                {
                    return OrderType.Service;
                }
                else if (Tour != null && Tour.Type == TourType.PrivateHire)
                {
                    return OrderType.PrivateHire;
                }
                else
                {
                    return IsGroup ? OrderType.RegularGroup : OrderType.Regular;
                }
            }
        }

        /// <summary>
        /// Ключ текущего шага
        /// </summary>
        public string CurrentStepName { get; set; }

        /// <summary>
        /// Статус заказа
        /// </summary>
        public OrderState? OrderState => (OrderState?)ProcessHelper.GetEnumItemByStepName(CurrentStepName);

        /// <summary>
        /// Свободные места.
        /// </summary>
        public int FreeCount => GuestCount - Seats.Count;

        /// <summary>
        /// Aктивный заказ
        /// </summary>
        public bool IsActive => OrderState != Enums.OrderState.Canceled && OrderState != Enums.OrderState.NotPaid;

        public Order()
        {
            Seats = new List<OrderSeat>();
            Menus = new List<OrderMenu>();
            Beverages = new List<OrderBeverage>();
            Surprises = new List<OrderSurprise>();
        }
    }
}
