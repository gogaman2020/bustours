using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Db.Repositories
{
    /// <summary>
    /// Интерфейс базового репозитория для вставки/обновления и удаления
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICrudRepository<T> : ICrudRepository<int, T>
    {
    }

    public interface ICrudRepository<TId, T>
    {
        Task<T> GetAsync(TId id);

        Task<T[]> GetAsync(params TId[] ids);

        Task<T> GetAsync(TId id, bool fillNested);

        Task<T[]> GetAsync(IEnumerable<TId> ids);

        Task<T[]> GetAsync(bool fillNested, params TId[] ids);

        Task<TId> SaveOrUpdateAsync(T entity);

        Task<TId[]> SaveOrUpdateAsync(IEnumerable<T> entities);

        Task DeleteAsync(T entity);

        Task DeleteAsync(IEnumerable<T> entities);
    }
}