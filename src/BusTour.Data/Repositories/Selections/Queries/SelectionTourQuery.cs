using Infrastructure.Db.SqlScriptManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusTour.Data.Repositories.Selections.Queries
{
    public class SelectionQuery
    {
        protected static readonly SqlScriptGetter<SelectionQuery> Getter = new SqlScriptGetter<SelectionQuery>();

        public static string SelectTourInfo(IEnumerable<string> fields) =>
           Getter.Get("SelectTourInfo", null, fields);
        public static string SelectBusObjectsPositions(IEnumerable<string> fields) =>
           Getter.Get("SelectBusObjectsPositions", null, fields);

        public object GetParams() => this;

        public int? TourId { get; set; }
    }
}
