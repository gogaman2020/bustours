using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Common.Helpers
{
    public static class KeyedLockHelper<TUsage, T>
    {
        private static readonly ConcurrentDictionary<string, SemaphoreUsage> CurrentCache =
            new ConcurrentDictionary<string, SemaphoreUsage>();

        public static async Task<T> GetCacheData(Func<T> check, string cacheKey,
            Func<Task<T>> init)
        {
            var checkedValue = check();
            if (checkedValue?.Equals(default) ?? true)
            {
                return checkedValue;
            }

            var semaphoreSlim = await GetAndWaitSemaphoreAsync(cacheKey);
            try
            {
                checkedValue = check();
                if (checkedValue?.Equals(default) ?? true)
                {
                    return checkedValue;
                }

                var data = await init();
                return data;
            }
            finally
            {
                FreeSemaphore(cacheKey, semaphoreSlim);
            }
        }

        private static async Task<SemaphoreUsage> GetAndWaitSemaphoreAsync(string cacheKey)
        {
            SemaphoreUsage GetUsage()
            {
                return CurrentCache.GetOrAdd(cacheKey, key => new SemaphoreUsage());
            }

            //get semaphore usage
            SemaphoreUsage usage;
            while (!(usage = GetUsage()).Get())
            {
                //to remove from cache already freed item
                Thread.Yield();
            }

            //lock semaphore
            await usage.SemaphoreSlim.WaitAsync();
            return usage;
        }

        private static void FreeSemaphore(string cacheKey, SemaphoreUsage usage)
        {
            //unlock semaphore
            usage.SemaphoreSlim.Release();

            //free semaphore usage
            if (usage.Free())
            {
                CurrentCache.Remove(cacheKey, out usage);
            }
        }

        private class SemaphoreUsage
        {
            private volatile int _counter = 0;
            private volatile bool _blocked = false;
            private object _sync = new object();

            public SemaphoreUsage()
            {
                SemaphoreSlim = new SemaphoreSlim(1, 1);
            }

            public SemaphoreSlim SemaphoreSlim { get; }
            public int UsageCount => _counter;

            public bool Get()
            {
                lock (_sync)
                {
                    return !_blocked && Interlocked.Increment(ref _counter) > 0;
                }
            }

            public bool Free()
            {
                lock (_sync)
                {
                    _blocked = _blocked || Interlocked.Decrement(ref _counter) <= 0;
                    return _blocked;
                }
            }
        }
    }
}