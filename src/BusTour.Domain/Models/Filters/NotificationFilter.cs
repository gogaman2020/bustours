using System;
using System.Collections.Generic;
using System.Text;

namespace BusTour.Domain.Models.Filters
{
    public class NotificationFilter
    {
        public int? Id { get; set; }
        public bool? IsSent { get; set; }

        public bool? HasEmail { get; set; }
    }
}
