using System;
using System.Collections.Concurrent;
using Infrastructure.Common.DI;
using Microsoft.Extensions.Options;

namespace Infrastructure.Common.Configs
{
    public static class Config
    {
        private static readonly ConcurrentDictionary<Type, object> Cache =
            new ConcurrentDictionary<Type, object>();

        public static T Get<T>()
            where T : class, new()
        {
            var scoped = (Scoped<IOptions<T>>) Cache.GetOrAdd(typeof(T), k => GetObj<T>());
            return scoped.ServiceRequired.Value;
        }

        private static Scoped<IOptions<T>> GetObj<T>()
            where T : class, new()
        {
            return IoC.GetRequiredService<ScopedWithFallback<IOptions<T>, IOptionsSnapshot<T>>>();
        }
    }
}