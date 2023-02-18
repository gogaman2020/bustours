using System;

namespace BusTour.Data.Repositories.PromoCodes
{
    public class PromoCodeFilter
    {
        /// <summary>
        /// Город
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Только активные
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Истекшие или измененные
        /// </summary>
        public bool? IsExpiredOrAmended { get; set; }

        /// <summary>
        /// Фильтры-помощники по статусу
        /// </summary>
        public bool? ApplyStatusFilter =>  ((IsActive ?? false) ^ (IsExpiredOrAmended ?? false)) ? true : (bool?)null;
        public bool? IsExpiredFilter => ApplyStatusFilter.HasValue && IsExpiredOrAmended == true ? true : (bool?)null;

        /// <summary>
        /// Текущее время в UTC
        /// </summary>
        public DateTime UtcDateTimeUtcNow => DateTime.UtcNow;

        public string SeriesNumber { get; set; }
    }
}
