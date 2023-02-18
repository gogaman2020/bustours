using BusTour.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusTour.Domain.Models.Order
{
    public class OrderPrivateHireModel
    {
        /// <summary>
        /// Идентификатор автобуса
        /// </summary>
        public int BusId { get; set; }

        /// <summary>
        /// Дата бронирования
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Бронь на весь день
        /// </summary>
        public bool IsAllDay { get; set; }

        /// <summary>
        /// Время начала брони
        /// </summary>
        public Time? TimeFrom { get; set; }

        /// <summary>
        /// Время окончания брони
        /// </summary>
        public Time? TimeTo { get; set; }

        /// <summary>
        /// Блокировать бронь для черновика
        /// </summary>
        public bool BlockBookingForDraft { get; set; }

        /// <summary>
        /// Время начала блокировки брони
        /// </summary>
        public Time? BlockBookingTimeFrom { get; set; }

        /// <summary>
        /// Время окончания блокировки брони
        /// </summary>
        public Time? BlockBookingTimeTo { get; set; }

        /// <summary>
        /// Точка отправления
        /// </summary>
        public string DeparturePoint { get; set; }

        /// <summary>
        /// Точка прибытия
        /// </summary>
        public string ArrivalPoint { get; set; }

        /// <summary>
        /// Идентификатор маршрута
        /// </summary>
        public int? RouteId { get; set; }

        /// <summary>
        /// Остановки
        /// </summary>
        public List<string> Stops { get; set; }

        /// <summary>
        /// Цена тура
        /// </summary>
        public decimal TourPrice { get; set; }

        /// <summary>
        /// Расчетное время начала брони
        /// </summary>
        public Time TimeFromCalculated => IsAllDay || !TimeFrom.HasValue ? new Time(0, 0, 0) : TimeFrom.Value;

        /// <summary>
        /// Расчетное время окончания брони
        /// </summary>
        public Time TimeToCalculated => IsAllDay || !TimeTo.HasValue ? new Time(23, 59, 59) : TimeTo.Value;

        public DateTime DepartureDateTime => TimeFromCalculated.AddToDate(Date);
        public DateTime ArrivalDateTime => TimeToCalculated.AddToDate(Date);
        public DateTime BlockBookingDateTimeFrom => new DateTime(Math.Max((BlockBookingTimeFrom?.AddToDate(Date) ?? DepartureDateTime).Ticks, DepartureDateTime.Ticks));
        public DateTime BlockBookingDateTimeTo => new DateTime(Math.Max((BlockBookingTimeTo?.AddToDate(Date) ?? ArrivalDateTime).Ticks, ArrivalDateTime.Ticks));
    }
}
