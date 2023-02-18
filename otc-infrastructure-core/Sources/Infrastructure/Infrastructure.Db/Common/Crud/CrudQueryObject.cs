using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Db.Common.Crud
{
    public class CrudQueryObject<TEntity, TCrudQuery> : CrudQueryObject<int, TEntity, TCrudQuery>
        where TEntity : IEntity
        where TCrudQuery : ICrudQuery, new()
    {
        public CrudQueryObject(TEntity entity, CrudOperation operation, bool useEntityForSelect = false) : base(entity, operation, useEntityForSelect)
        {
        }
    }

    public class CrudQueryObject<TId, TEntity, TCrudQuery> : BaseQueryObject<TId, TEntity>
        where TEntity : IEntity<TId>
        where TCrudQuery : ICrudQuery, new()
    {
        private static readonly Func<ICrudQuery, Func<IEnumerable<string>, string>>[] Operations;

        static CrudQueryObject()
        {
            Operations = new Func<ICrudQuery, Func<IEnumerable<string>, string>>[4];
            Operations[(int) CrudOperation.Select] = q => q.GetSelect;
            Operations[(int) CrudOperation.Insert] = q => q.GetInsert;
            Operations[(int) CrudOperation.Delete] = q => q.GetDelete;
            Operations[(int) CrudOperation.Update] = q => q.GetUpdate;
        }

        private readonly ICrudQuery _crudQuery;
        private readonly CrudOperation _operation;
        private readonly TId[] _ids;
        private readonly bool _useEntityForSelect;

        public CrudQueryObject(TEntity entity, CrudOperation operation, bool useEntityForSelect = false) : this(entity, operation, new TCrudQuery(), useEntityForSelect)
        {
        }

        public CrudQueryObject(TEntity entity, CrudOperation operation, ICrudQuery crudQuery, bool useEntityForSelect = false) : base(entity)
        {
            _crudQuery = crudQuery;
            _operation = operation;
            _ids = null;
            _useEntityForSelect = useEntityForSelect;
        }

        public CrudQueryObject(IEnumerable<TId> ids, ICrudQuery crudQuery = default) : base(default)
        {
            _crudQuery = crudQuery ?? new TCrudQuery();
            _operation = CrudOperation.Select;
            _ids = ids.ToArray();
        }

        public override IReadOnlyDictionary<string, object> Params => _operation == CrudOperation.Select && !_useEntityForSelect
            //фильтруем только Id для Select
            ? (_ids == null
                ? new Dictionary<string, object>
                {
                    [nameof(IEntity<TId>.Id)] = new[] {(TId) base.Params[nameof(IEntity<TId>.Id)]}
                }
                : new Dictionary<string, object>
                {
                    [nameof(IEntity<TId>.Id)] = _ids
                })
            : base.Params;

        public override string GetQuery()
        {
            var sql = Operations[(int) _operation](_crudQuery)(Params.Keys);
            return sql;
        }
    }
}