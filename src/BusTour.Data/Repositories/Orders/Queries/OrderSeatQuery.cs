using BusTour.Domain.Entities;
using System.Collections.Generic;

namespace BusTour.Data.Repositories.Orders.Queries
{
    public class OrderSeatQuery : BaseOrderQuery<OrderSeat, ClientQuery>
    {
        public static string GetSeatsByTables(IEnumerable<string> fields) =>
           Getter.Get("GetSeatsByTables", null, fields);

        public static string SelectOrderSeat(IEnumerable<string> fields) =>
           Getter.Get("SelectOrderSeat", null, fields);

        public static string AllGuestHasCome(IEnumerable<string> fields) =>
           Getter.Get("AllGuestHasCome", null, fields);

        public List<int> TableIds { get; set; }

        public List<int> OrderIds { get; set; }
    }
}
