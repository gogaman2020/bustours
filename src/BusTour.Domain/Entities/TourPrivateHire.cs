using BusTour.Domain.Models;
using Infrastructure.Db.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusTour.Domain.Entities
{
    public class TourPrivateHire: BaseEntity
    {
        /// <summary>
        /// Идентификатор тура
        /// </summary>
        public int? TourId { get; set; }

        /// <summary>
        /// Длительность
        /// </summary>
        public TimeSpan? Duration { get; set; }

        /// <summary>
        /// Время начала блокировки брони
        /// </summary>
        public DateTime BlockBookingDateFrom { get; set; }

        /// <summary>
        /// Время окончания блокировки брони
        /// </summary>
        public DateTime BlockBookingDateTo { get; set; }

        /// <summary>
        /// Блокировать бронь для черновика
        /// </summary>
        public bool BlockBookingForDraft { get; set; }

        /// <summary>
        /// Точка отправления
        /// </summary>
        public string DeparturePoint { get; set; }

        /// <summary>
        /// Точка прибытия
        /// </summary>
        public string ArrivalPoint { get; set; }

        /// <summary>
        /// Остановки
        /// </summary>
        public List<string> Stops { get; set; }

        /// <summary>
        /// Цена частного тура
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Количество гостей
        /// </summary>
        public int GuestCount { get; set; }

        [IgnoreField]
        public Tour Tour { get; set; }
    }
}
