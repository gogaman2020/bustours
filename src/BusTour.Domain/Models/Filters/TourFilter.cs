using BusTour.Domain.Enums;
using BusTour.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusTour.Domain.Models.Filters
{
    public class TourFilter
    {
        public IEnumerable<int> Ids { get; set; }

        public DateTime? DepartureDateFrom { get; set; }

        public DateTime? DepartureDateTo { get; set; }

        public DateTime? ArrivalDateFrom { get; set; }

        public DateTime? ArrivalDateTo { get; set; }

        public DateTime? BlockBookingDateFromEnd { get; set; }

        public DateTime? BlockBookingDateToStart { get; set; }

        public IEnumerable<TourType> TourTypes { get; set; }

        public int? CityId { get; set; }

        public int? RouteId { get; set; }

        public int? BusId { get; set; }

        public int? Limit { get; set; }

        public int? Offset { get; set; }

        public bool? HasOrders { get; set; }

        public bool? WithoutOrders => HasOrders.HasValue ? !HasOrders : null;
        
        public string? Date { get; set; }

        /// <remarks>В скриптах для поиска по статусу сравнивать это поле с tourstate.currentstepname</remarks>
        public IEnumerable<string> TourProcessStates => ProcessHelper.GetStepNamesByEnumItems(States);

        public IEnumerable<TourState> States { get; set; }
    }
}
