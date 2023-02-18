using BusTour.Domain.Entities;
using System.Collections.Generic;

namespace BusTour.Data.Repositories.Orders.Queries
{
    public class ClientQuery : BaseOrderQuery<Client, ClientQuery>
    {
        public static string UpsertCommand(IEnumerable<string> fields) =>
           Getter.Get("UpsertClient", null, fields);
    }
}
