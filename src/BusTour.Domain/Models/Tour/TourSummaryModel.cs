using BusTour.Domain.Enums;
using System;
using System.Collections.Generic;

namespace BusTour.Domain.Models.Tour
{
    /// <summary>
    /// Основная информация по туру
    /// </summary>
    public class TourSummaryModel
    {
        /// <summary>
        /// Идентификатор тура
        /// </summary>
        public int TourId { get; set; }

        /// <summary>
        /// Номер тура
        /// </summary>
        public string TourNumber { get; set; }

        /// <summary>
        /// Идентификатор города маршрута
        /// </summary>
        public int? CityId { get; set; }

        /// <summary>
        /// Тип тура
        /// </summary>
        public TourType TourType { get; set; }

        /// <summary>
        /// Наименование маршрута
        /// </summary>
        public Dictionary<string, string> Itinerary { get; set; }

        /// <summary>
        /// Статус тура
        /// </summary>
        public TourState? TourState { get; set; }

        /// <summary>
        /// Блокировка публичного бронирования
        /// </summary>
        public bool PublicBookingBlock => TourType != TourType.Regular;

        /// <summary>
        /// Дата и время отправления
        /// </summary>
        public DateTime DepartureDateTime { get; set; }

        /// <summary>
        /// Дата и время прибытия
        /// </summary>
        public DateTime ArrivalDateTime { get; set; }

        /// <summary>
        /// Продолжительность тура
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Есть ли конфликты
        /// </summary>
        public bool Conflict { get; set; }

        /// <summary>
        /// Количество мест
        /// </summary>
        public int SeatsCount { get; set; }

        /// <summary>
        /// Количество занятых мест
        /// </summary>
        public int? Occupaid { get; set; }

        /// <summary>
        /// Количество использованных сертификатов
        /// </summary>
        public int? UsedGiftsCount { get; set; }

        /// <summary>
        /// Количество использованных промокодов
        /// </summary>
        public int? UserPromoCodesCount { get; set; }

        /// <summary>
        /// Есть ли дополнительное продовольствие
        /// </summary>
        public bool Extras { get; set; }

        /// <summary>
        /// Количество оплаченных мест
        /// </summary>
        public int TourPaymentInformation { get; set; }

        /// <summary>
        /// Размер НДС
        /// </summary>
        public decimal VatPrice { get; set; }

        /// <summary>
        /// Стоимость тура с НДС
        /// </summary>
        public decimal TotalPriceWithVat => TotalPrice + VatPrice;

        /// <summary>
        /// Стоимость тура с НДС
        /// </summary>
        public decimal TotalPrice { get; set; }
    }
}
