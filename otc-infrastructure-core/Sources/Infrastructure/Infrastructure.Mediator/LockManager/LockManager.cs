using Infrastructure.Common.DI;
using Infrastructure.Common.Helpers;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Mediator.LockManager
{
    /// <summary>
    /// todo для увеличения производительности
    /// Задача разруливания блокировок между параллельными потоками по приоритетам захвата из контекста
    /// пока заглушка и строго последовательное испольнение в медиаторе
    /// </summary>
    [InjectAsSingleton]
    public class LockManager : ILockManager
    {
        public async Task<bool> WithLockAsync(string key, Func<Task> func)
        {
            return await KeyedLockHelper<LockManager, bool>.GetCacheData(
                () => false,
                key,
                async () =>
                {
                    await func();
                    return true;
                });
        }
    }
}
