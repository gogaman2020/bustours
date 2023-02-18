using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Common.DI;
using Infrastructure.Db.Audit;
using Infrastructure.Db.Common;
using Infrastructure.Db.Common.Crud;

namespace Infrastructure.Db.Repositories
{
    [Obsolete]
    public class UniversalCrudRepository
    {
        protected readonly IDb _db;
        protected readonly Scoped<IRevisionManager> _revisionManager;

        public UniversalCrudRepository(IDb db, Scoped<IRevisionManager> revisionManager)
        {
            _db = db;
            _revisionManager = revisionManager;
        }

        // без параметров смысла не имеет - всю таблицу тащить...
        // public async Task<IEnumerable<TEntity>> SelectAsync<TEntity, TCrudQuery>()
        //     where TEntity : IEntity, new()
        //     where TCrudQuery : ICrudQuery, new()
        // {
        //     return await _db.QueryAsync<TEntity>(new CrudQueryObject<TEntity, TCrudQuery>(CrudOperation.Select));
        // }

        public Task<int> SaveOrUpdateAsync<TEntity, TCrudQuery>(TEntity entity)
            where TEntity : IEntity, new()
            where TCrudQuery : ICrudQuery, new()
            => Repository<TEntity, TCrudQuery>().SaveOrUpdateAsync(entity);

        public Task<int[]> SaveOrUpdateAsync<TEntity, TCrudQuery>(IEnumerable<TEntity> entities)
            where TEntity : IEntity, new()
            where TCrudQuery : ICrudQuery, new()
            => Repository<TEntity, TCrudQuery>().SaveOrUpdateAsync(entities);

        public Task DeleteAsync<TEntity, TCrudQuery>(TEntity entity)
            where TEntity : IEntity, new()
            where TCrudQuery : ICrudQuery, new()
            => Repository<TEntity, TCrudQuery>().DeleteAsync(entity);
        
        public Task DeleteAsync<TEntity, TCrudQuery>(IEnumerable<TEntity> entities)
            where TEntity : IEntity, new()
            where TCrudQuery : ICrudQuery, new()
            => Repository<TEntity, TCrudQuery>().DeleteAsync(entities);

        private CrudRepository<TEntity, TCrudQuery> Repository<TEntity, TCrudQuery>() 
            where TEntity : IEntity, new()
            where TCrudQuery : ICrudQuery, new()
            => new CrudRepository<TEntity, TCrudQuery>(_db, _revisionManager);
    }
}
