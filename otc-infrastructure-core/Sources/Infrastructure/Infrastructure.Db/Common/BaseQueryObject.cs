using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Db.Common
{
    public abstract class BaseQueryObject<TEntity> : BaseQueryObject<int, TEntity>
        where TEntity : IEntity
    {
        protected BaseQueryObject(TEntity entity) : base(entity)
        {
        }
    }

    public abstract class BaseQueryObject<TId, TEntity> : IQueryObject
        where TEntity : IEntity<TId>
    {
        public virtual IReadOnlyDictionary<string, object> Params { get; }

        protected BaseQueryObject(TEntity entity)
        {
            Params = EntityExtensions.GetQueryParameters(entity)
                ?.ToDictionary(i=>i.PropName, i=>i.Value);
        }

        public object GetParams() => Params;

        public abstract string GetQuery();
    }
}