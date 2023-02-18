using Infrastructure.Db.Common.Crud;
using System;
using System.Collections.Generic;

namespace Infrastructure.Process.Repositories.Queries
{
    public class UpsertProcessStateQuery<T> : CrudQuery<T, UpsertProcessStateQuery<T>>
        where T : class
    {
        public static string SelectCommand(IEnumerable<string> fields, Dictionary<string, string> values) =>
           Getter.Get("SelectProcessState", null, fields, values);

        protected static readonly string UpsertName = $"UpsertProcessState";

        public static string UpsertCommand(IEnumerable<string> fields, Dictionary<string, string> values) =>
           Getter.Get(UpsertName, null, fields, values);

        public T State { get; set; }

        protected override string ResolveName(CrudOperation operation)
        {
            switch (operation)
            {
                case CrudOperation.Insert:
                case CrudOperation.Update:
                    return UpsertName;

                default:
                    return base.ResolveName(operation);
            }
        }
    }
}
