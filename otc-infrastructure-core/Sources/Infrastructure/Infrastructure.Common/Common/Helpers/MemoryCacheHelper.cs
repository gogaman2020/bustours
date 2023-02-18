using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Common.Helpers
{
    public class CacheData
    {
        /// <summary>
        /// Время жизни по умолчанию
        /// </summary>
        private const int ExpirationTime = 60 * 10;

        public CacheData(object data)
        {
            Data = data;
            DateTimeOffSet = DateTime.Now.AddSeconds(ExpirationTime);
        }

        public CacheData(object data, DateTimeOffset dateTimeOffSet)
        {
            Data = data;
            DateTimeOffSet = dateTimeOffSet;
        }

        public object Data { get; protected set; }

        public DateTimeOffset DateTimeOffSet { get; protected set; }
    }

    public static class MemoryCacheHelper
    {
        public static async Task<object> GetCacheData(this IMemoryCache memoryCache, string cacheKey,
            Func<Task<CacheData>> init)
        {
            return await KeyedLockHelper<CacheData, object>.GetCacheData(
                () =>
                {
                    var cacheData = memoryCache.Get(cacheKey);
                    return cacheData;
                },
                cacheKey,
                async () =>
                {
                    var data = await init();
                    memoryCache.Set(cacheKey, data.Data, data.DateTimeOffSet);
                    return data.Data;
                });
        }
    }
}