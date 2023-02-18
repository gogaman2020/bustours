using BusTour.Domain.Enums;
using Infrastructure.Db.Common;
using System;
using System.Collections.Generic;
using BusTour.Domain.Helpers;

namespace BusTour.Domain.Entities
{
    public class CurrentBookingGridModel : BaseEntity
    {
        public string Number { get; set; }
        public DateTime Departure { get; set; }
        public int? TourType { get; set; }
        public Dictionary<string, string> Itinerary { get; set; }
        public string PrivateHireComment { get; set; }
        public TimeSpan Duration { get; set; }
        public Dictionary<string, string> City { get; set; }
        public TourState? TourState => (TourState?)ProcessHelper.GetEnumItemByStepName(CurrentStepName);
        public string CurrentStepName { get; set; }
        public int? GuestsNumber { get; set; }
        public int? SeatsNumber { get; set; }
        public bool? Conflicts { get; set; }
        public bool? Extras { get; set; }
        public int? Paid { get; set; }
        public int? Waiting { get; set; }
        public bool HasGroupOrder { get; set; }
    }
}