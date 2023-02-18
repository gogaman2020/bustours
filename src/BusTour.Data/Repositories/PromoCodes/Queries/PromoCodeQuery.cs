using BusTour.Domain.Entities;
using Infrastructure.Db.Common.Crud;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusTour.Data.Repositories.PromoCodes.Queries
{
    public class PromoCodeQuery : CrudQuery<PromoCode, PromoCodeQuery>
    {
        public static string SelectGridByFilter(IEnumerable<string> fields, Dictionary<string, string> values) =>
            Getter.Get("SelectPromoCodeGridByFilter", new[] {"Body", "From", "ByFilter" }, fields, values);

        public static string SelectCount(IEnumerable<string> fields, Dictionary<string, string> values) =>
            Getter.Get("SelectPromoCodeGridByFilter", new[] { "Count", "From", "ByFilter" }, fields, values);

        public static string SelectByFilter(IEnumerable<string> fields) =>
            Getter.Get("SelectPromoCode", null, fields);
    }
}
