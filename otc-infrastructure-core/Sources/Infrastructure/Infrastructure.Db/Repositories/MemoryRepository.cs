using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Common.Json;
using Infrastructure.Db.Common;

namespace Infrastructure.Db.Repositories
{
    public class MemoryRepository<T> : MemoryRepository<int, T>, ICrudRepository<T>
        where T : class, IEntity
    {
        private int sequence = 0;
        protected override int GenerateNextId()
        {
            return Interlocked.Increment(ref sequence);
        }
    }

    public abstract class MemoryRepository<TId, T> : ICrudRepository<TId, T>
        where T : class, IEntity<TId>
    {
        private readonly ConcurrentDictionary<TId, T> _data = new ConcurrentDictionary<TId, T>();

        protected abstract TId GenerateNextId();
        public T[] GetAll()
        {
            return _data.Values.ToArray();
        }

        public Task<T> GetAsync(TId id)
        {
            var result = _data.TryGetValue(id, out var data)
                ? Task.FromResult(CopyViaJson(data))
                : Task.FromResult<T>(default);
            return result;
        }

        public Task<T[]> GetAsync(params TId[] ids)
        {
            return GetAsync((IEnumerable<TId>) ids);
        }

        public Task<T[]> GetAsync(IEnumerable<TId> ids)
        {
            var result = ids
                .Select(t => GetAsync((TId) t).Result)
                .Where(d => d != null)
                .ToArray();
            return Task.FromResult(result);
        }

        public virtual Task<TId> SaveOrUpdateAsync(T entity)
        {
            if (entity.Id?.Equals(default(TId)) ?? true)
            {
                entity.Id = GenerateNextId();
            }

            var json = CopyViaJson(entity);
            _data.AddOrUpdate(entity.Id, id => json, (id, old) => json);
            return Task.FromResult(entity.Id);
        }

        public Task<TId[]> SaveOrUpdateAsync(IEnumerable<T> entities)
        {
            var result = entities
                .Select(e => SaveOrUpdateAsync(e).Result)
                .ToArray();
            return Task.FromResult(result);
        }

        public Task DeleteAsync(T entity)
        {
            _data.TryRemove(entity.Id, out _);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(IEnumerable<T> entities)
        {
            var _ = entities
                .Select(DeleteAsync)
                .ToArray();
            return Task.CompletedTask;
        }

        protected T FilterOne<TFilter>(Predicate<T, TFilter> predicate, TFilter filter)
        {
            var result = _data.Values.FirstOrDefault(t => predicate(t, filter));
            return CopyViaJson(result);
        }

        protected IEnumerable<T> FilterAll<TFilter>(Predicate<T, TFilter> predicate, TFilter filter)
        {
            return _data.Values.Where(t => predicate(t, filter)).Select(CopyViaJson);
        }

        protected static Predicate<T, TFilter> AsPredicate<TFilter>()
        {
            return (t, f) => true;
        }

        protected static PredicateBuilder<T, TFilter> AsPredicateBuilder<TFilter>()
        {
            return new PredicateBuilder<T, TFilter>();
        }

        protected virtual string ToJson(T entity)
        {
            return entity?.ToJson();
        }

        protected virtual T FromJson(string json)
        {
            return json.FromJson<T>();
        }

        private T CopyViaJson(T obj)
        {
            return obj == null
                ? null
                : FromJson(ToJson(obj));
        }

        public Task<T> GetAsync(TId id, bool fillNested)
        {
            throw new System.NotImplementedException();
        }

        public Task<T[]> GetAsync(bool fillNested, params TId[] ids)
        {
            throw new System.NotImplementedException();
        }
    }
}