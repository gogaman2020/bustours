using BusTour.Domain.Entities;
using Infrastructure.Db.Common.Crud;
using Infrastructure.Db.SqlScriptManagement;
using System.Collections.Generic;

namespace BusTour.Data.Repositories.BusRepository.Queries
{
    public class BusQuery: CrudQuery<Bus, BusQuery>
    {
        public static string SelectBus(IEnumerable<string> fields) =>
            Getter.Get("SelectBus", null, fields);

        public static string SelectSeat(IEnumerable<string> fields) =>
            Getter.Get("SelectSeat", null, fields);
    }
}