using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Common.DI;
using Infrastructure.Common.Helpers;
using Infrastructure.Db.Audit;
using Infrastructure.Db.Common;
using Infrastructure.Db.Common.Crud;

namespace Infrastructure.Db.Repositories
{
    public class CrudRepository<TEntity, TCrudQuery> : CrudRepository<int, TEntity, TCrudQuery>, ICrudRepository<TEntity>
        where TEntity : IEntity, new()
        where TCrudQuery : ICrudQuery, new()
    {
        public CrudRepository(){}
        public CrudRepository(IDb db, Scoped<IRevisionManager> revisionManager)
            : base(db, revisionManager)
        {
        }
    }

    public class CrudRepository<TId, TEntity, TCrudQuery> : ICrudRepository<TId, TEntity>
        where TEntity : IEntity<TId>, new()
        where TCrudQuery : ICrudQuery, new()
    {
        protected readonly IDb _db;
        protected readonly DbAudit<TId, TEntity> _audit;
        protected readonly ICrudQuery _queries;

        public CrudRepository()
            : this(IoC.GetRequiredService<IDb>(), IoC.GetRequiredService<Scoped<IRevisionManager>>())
        {
        }

        public CrudRepository(IDb db, Scoped<IRevisionManager> revisionManager)
        {
            _db = db;
            _audit = new DbAudit<TId, TEntity>(db, revisionManager);
            _queries = new TCrudQuery();
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            if (entity != null) //Если entity == null -> удаляются все записи из таблицы.
            {
                await _db.ExecuteAsync(async (commands, ct) =>
                {
                    var revision = await _audit.WithRevisionAsync();
                    await ProcessInternalAsync(entity, _db, CrudOperation.Delete, revision);
                });
            }
        }

        public virtual async Task DeleteAsync(IEnumerable<TEntity> entities)
        {
            await _db.ExecuteAsync(async (commands, ct) =>
            {
                var revision = await _audit.WithRevisionAsync();
                foreach (var entity in entities)
                {
                    await ProcessInternalAsync(entity, _db, CrudOperation.Delete, revision);
                }
            }, true);
        }

        public virtual async Task<TEntity> GetAsync(TId id)
        {
            return (await GetAsync(new[] {id})).FirstOrDefault();
        }

        public virtual async Task<TEntity> GetAsync(TId id, bool fillNested)
        {
            return (await GetAsync(fillNested, new[] { id })).FirstOrDefault();
        }

        public virtual async Task<TEntity[]> GetAsync(params TId[] ids)
        {
            TEntity[] entities = null; 
            await _db.ExecuteAsync(async (commands, ct) =>
            {
                var queryObject = GetDefaultQueryObject(ids);
                entities = (await commands.QueryAsync<TEntity>(queryObject))
                    .ToArray();
            });
            await FillNestedAsync(entities);
            return entities ?? Array.Empty<TEntity>();
        }

        public virtual async Task<TEntity[]> GetAsync(bool fillNested, params TId[] ids)
        {
            TEntity[] entities = null;
            await _db.ExecuteAsync(async (commands, ct) =>
            {
                var queryObject = GetDefaultQueryObject(ids);
                entities = (await commands.QueryAsync<TEntity>(queryObject))
                    .ToArray();
            });
            if (fillNested)
            {
                await FillNestedAsync(entities);
            }
            return entities ?? Array.Empty<TEntity>();
        }

        protected virtual Task FillNestedAsync(TEntity[] entities)
        {
            return Task.CompletedTask;
        }
        
        public Task<TEntity[]> GetAsync(IEnumerable<TId> ids)
        {
            return GetAsync(ids.ToArray());
        }

        public virtual async Task<TId> SaveOrUpdateAsync(TEntity entity)
        {
            await _db.ExecuteAsync(async (commands, ct) =>
                {
                    var revision = await _audit.WithRevisionAsync();
                    await SaveOrUpdateInternalAsync(entity, _db, revision);
                },
                true);
            return entity.Id;
        }

        public virtual async Task<TId[]> SaveOrUpdateAsync(IEnumerable<TEntity> entities)
        {
            return await _db.QueryAsync(async (commands, ct) =>
            {
                var ids = new List<TId>();
                var revision = await _audit.WithRevisionAsync();
                foreach (var entity in entities)
                {
                    await SaveOrUpdateInternalAsync(entity, commands, revision);
                    ids.Add(entity.Id);
                }

                return ids.ToArray();
            }, true);
        }

        private async Task SaveOrUpdateInternalAsync(TEntity entity, IDbCommands commands,
            DbAudit<TId, TEntity>.Revision revision)
        {
            var needSave = await HasChangesAsync(entity);
            if (needSave)
            {
                var crudOperation = (entity.Id?.Equals(default(TId)) ?? true) ? CrudOperation.Insert : CrudOperation.Update;
                await ProcessInternalAsync(entity, commands, crudOperation, revision);
                await AfterUpdateAsync(entity);
            }
        }

        protected virtual Task<bool> HasChangesAsync(TEntity entity)
        {
            return Task.FromResult(true);
        }

        protected virtual Task AfterUpdateAsync(TEntity entity)
        {
            return Task.CompletedTask;
        }

        private async Task ProcessInternalAsync(TEntity entity, IDbCommands commands, CrudOperation crudOperation,
            DbAudit<TId, TEntity>.Revision revision)
        {
            var queries = GetQueries(entity, crudOperation);
            foreach (var tuple in queries)
            {
                var result = await commands.QueryFirstOrDefaultAsync<dynamic>(tuple.Query);
                tuple.Handler?.Invoke(result as IDictionary<string, object>, entity);
            }

            if (revision != null)
            {
                await revision.SaveHistoryAsync(entity, crudOperation);
            }
        }

        protected IQueryObject GetDefaultQueryObject(TEntity entity, CrudOperation crudOperation)
        {
            return new CrudQueryObject<TId, TEntity, TCrudQuery>(entity, crudOperation, _queries);
        }
        protected IQueryObject GetDefaultQueryObject(TId[] ids)
        {
            return new CrudQueryObject<TId, TEntity, TCrudQuery>(ids, _queries);
        }

        protected Action<IDictionary<string, object>, TEntity> GetDefaultQueryAction(TEntity entity,
            CrudOperation crudOperation)
        {
            if (typeof(TId) != typeof(int))
            {
                return null;
            }

            Action<IDictionary<string, object>, TEntity> action = null;

            if (crudOperation == CrudOperation.Insert)
            {
                action = (result, data) => data.Id = (TId) (object) result.FirstAsInt();
            }

            return action;
        }

        protected (IQueryObject Query, Action<IDictionary<string, object>, TEntity> Handler) GetDefaultQuery(
            TEntity entity, CrudOperation crudOperation)
        {
            return (GetDefaultQueryObject(entity, crudOperation), GetDefaultQueryAction(entity, crudOperation));
        }

        protected virtual IEnumerable<IQueryObject> GetQueryObjects(TEntity entity, CrudOperation crudOperation)
        {
            yield return GetDefaultQueryObject(entity, crudOperation);
        }

        protected virtual IEnumerable<(IQueryObject Query, Action<IDictionary<string, object>, TEntity> Handler)>
            GetQueries(TEntity entity, CrudOperation crudOperation)
        {
            var queries = GetQueryObjects(entity, crudOperation);
            foreach (var query in queries)
            {
                yield return (query, GetDefaultQueryAction(entity, crudOperation));
            }
        }
        
        protected async Task FillAsync<TParent, TChildId, TChild>(TParent[] parents, Func<TParent, TChildId> getId,
            Action<TParent, TChild> fill, ICrudRepository<TChildId, TChild> repo)
            where TChild : IEntity<TChildId>
        {
            var ids = parents
                .Select(l => getId(l))
                .Where(id => !(id?.Equals(default(TChildId)) ?? true))
                .Distinct()
                .ToArray();

            if (ids.Length == 0)
            {
                return;
            }
            
            var dic = (await repo.GetAsync(ids))
                .ToDictionary(o => o.Id);

            foreach (var parent in parents)
            {
                if (dic.TryGetValue(getId(parent), out var child))
                {
                    fill(parent, child);
                }
            }
        }
    }
}