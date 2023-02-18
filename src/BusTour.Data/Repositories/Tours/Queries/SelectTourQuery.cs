using Infrastructure.Db.SqlScriptManagement;
using System.Collections.Generic;

namespace BusTour.Data.Repositories.Tours.Queries
{
    public class SelectTourQuery
    {
        protected static readonly SqlScriptGetter<SelectTourQuery> Getter = new SqlScriptGetter<SelectTourQuery>();

        public static string SelectRouteInfo(IEnumerable<string> fields) =>
            Getter.Get("SelectRouteInfo", null , fields);

        public static string SelectTours(IEnumerable<string> fields) =>
            Getter.Get("SelectTours", null, fields);

        public static string SelectTourNested(IEnumerable<string> fields) =>
            Getter.Get("SelectTourNested", null, fields);

        public static string SelectMenus(IEnumerable<string> fields) =>
            Getter.Get("SelectMenus", null, fields);

        public static string SelectBeverages(IEnumerable<string> fields) =>
            Getter.Get("SelectBeverages", null, fields);

        public static string SelectAllergies(IEnumerable<string> fields) =>
            Getter.Get("SelectAllergies", null, fields);

        public static string SelectSurprises(IEnumerable<string> fields) =>
            Getter.Get("SelectSurprises", null, fields);

        public static string SelectCities(IEnumerable<string> fields) =>
            Getter.Get("SelectCity", null, fields);

        public static string SelectRoute(IEnumerable<string> fields) =>
            Getter.Get("SelectRoute", null, fields);

        public static string SelectPrivateHire(IEnumerable<string> fields) =>
            Getter.Get("SelectPrivateHire", null, fields);

        public static string SelectServiceMaintenance(IEnumerable<string> fields) =>
            Getter.Get("SelectServiceMaintenance", null, fields);

        public static string SelectByFilter(IEnumerable<string> fields) => 
            Getter.Get("SelectTour", null, fields);

        public static string SelectTourByFilter(IEnumerable<string> fields) =>
            Getter.Get("SelectTourById", null, fields);

        public static string SelectTourFB(IEnumerable<string> fields) =>
            Getter.Get("SelectTourFB", null, fields);

        public static string SelectTourGuests(IEnumerable<string> fields) =>
            Getter.Get("SelectTourGuests", null, fields);
        public static string SelectTourGroupOrder(IEnumerable<string> fields) =>
            Getter.Get("SelectTourGO", null, fields);

        public static string SelectTourSeats(IEnumerable<string> fields) =>
            Getter.Get("SelectTourSeats", null, fields);

        public static string SelectTourOrderMenu(IEnumerable<string> fields) =>
            Getter.Get("SelectTourOrderMenu", null, fields);

        public static string SelectTourOrderBeverage(IEnumerable<string> fields) =>
            Getter.Get("SelectTourOrderBeverage", null, fields);

        public static string SelectTourOrderExtraMenu(IEnumerable<string> fields) =>
            Getter.Get("SelectTourOrderExtraMenu", null, fields);

        public static string SelectTourOrderExtraBeverage(IEnumerable<string> fields) =>
            Getter.Get("SelectTourOrderExtraBeverage", null, fields);

        public static string SelectPrivateTourInfo(IEnumerable<string> fields) =>
            Getter.Get("SelectPrivateTourInfo", null, fields);

        public int? Id { get; set; }

        public string UserName { get; set; }
    }
}