using Infrastructure.Db.Common.Crud;
using System.Collections.Generic;

namespace BusTour.Data.Repositories.Orders.Queries
{
    public abstract class BaseOrderQuery<TEntity, TQuery>: CrudQuery<TEntity, TQuery>
        where TEntity : class
        where TQuery : class
    {
        public static string Insert(IEnumerable<string> fields) => Getter.Get(InsertName, null, fields);

        public static string Delete(IEnumerable<string> fields) => Getter.Get(DeleteName, null, fields);

        public TEntity Entity { get; set; }

        public int OrderId { get; set; }
    }
}
