using BusTour.Domain.Models.Order;
using System.Collections.Generic;

namespace BusTour.Domain.Models.Tour
{
    /// <summary>
    /// Модель информации о туре (/tour-information/id)
    /// </summary>
    public class TourInformationModel
    {
        /// <summary>
        /// Основная информация о туре
        /// </summary>
        public TourSummaryModel TourSummary { get; set; }

        public TourOrderGridModel[] TourOrders { get; set; }

        /// <summary>
        /// Конфликты тура
        /// </summary>
        public OrdersConflictsResponse TourConflicts { get; set; }
    }
}
