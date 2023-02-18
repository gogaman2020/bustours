using BusTour.Domain.Entities;
using System.Collections.Generic;

namespace BusTour.Domain.Models
{
    public class RouteInfo
    {
        public Route Route { get; set; }

        public List<Domain.Entities.Tour> Tours { get; set; }
    }
}
