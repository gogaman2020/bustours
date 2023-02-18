using BusTour.Domain.Enums;
using Infrastructure.Db.Common;
using BusTour.Domain.Helpers;
using Infrastructure.Process;
using System;
using System.Collections.Generic;

namespace BusTour.Domain.Entities
{
    public class TourGridModel : BaseEntity
    {
        public List<TourInfoModel> ToursInfo { get; set; }
        public List<OrderMenu> Menus { get; set; }

        public List<OrderBeverage> Beverages { get; set; }
        public List<OrderMenu> ExtraMenus { get; set; }
        public List<OrderBeverage> ExtraBeverages { get; set; }

        public List<PrivateTourInfo> PrivateTourInfo { get; set; }
    }

    public class TourInfoModel : BaseEntity
    {
        public DateTime Departure { get; set; }
        public TourType? TourType { get; set; }
        public Dictionary<string, string> Itinerary { get; set; }
        public TimeSpan Duration { get; set; }
        public Dictionary<string, string> City { get; set; }
        public TourState? Status => (TourState?)ProcessHelper.GetEnumItemByStepName(CurrentStepName);
        public string CurrentStepName { get; set; }
        public int GuestsNumber { get; set; }
        public string Number { get; set; }

    }

    public class PrivateTourInfo : BaseEntity
    {
        public string comment { get; set; }
        public string specialRequests { get; set; }
    }
}
