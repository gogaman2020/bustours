using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using System;
using System.Collections.Generic;

namespace BusTour.Data.Repositories.Orders.Queries
{
    public class OrderQuery:BaseOrderQuery<Order, OrderQuery>
    {
        public static string SelectLockedSeats(IEnumerable<string> fields) =>
           Getter.Get("SelectLockedSeats", null, fields);

        public static string SelectByFilter(IEnumerable<string> fields) =>
           Getter.Get("SelectOrder", null, fields);

        public int? Id { get; set; }

        public int? TourId { get; set; }

        public short? NewState { get; set; }

        public DateTime? StartDate { get; set; }

        public short? OldState { get; set; }
    }
}
