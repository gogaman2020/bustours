using System;
using System.Collections.Generic;
using System.Text;

namespace BusTour.Domain.Models.Filters
{
    public class SeatFilter
    {
        public IEnumerable<int> Ids { get; set; }
        public IEnumerable<int> BusIds { get; set; }
    }
}
