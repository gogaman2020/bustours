using BusTour.Domain.Entities;
using Infrastructure.Db.Common.Crud;
using Infrastructure.Db.SqlScriptManagement;
using System.Collections.Generic;

namespace BusTour.Data.Repositories.Tours.Queries
{
    public class TourQuery: CrudQuery<Tour, TourQuery>
    {
        public static string SelectByFilter(IEnumerable<string> fields) =>
            Getter.Get(SelectName, null, fields);

        public static string SelectTourMenus(IEnumerable<string> fields) =>
            Getter.Get("SelectTourMenus", null, fields);

        public static string SelectTourBeverages(IEnumerable<string> fields) =>
            Getter.Get("SelectTourBeverages", null, fields);

        public static string DeleteTourMenus => Getter.Get("DeleteTourMenus");

        public static string DeleteTourBeverages => Getter.Get("DeleteTourBeverages");

        public static string InsertPrivateHire => Getter.Get("InsertPrivateHire");

        public static string UpdatePrivateHire => Getter.Get("UpdatePrivateHire");

        public static string InsertServiceMaintenance => Getter.Get("InsertServiceMaintenance");

        public static string UpdateServiceMaintenance => Getter.Get("UpdateServiceMaintenance");
    }
}