using System;
using System.Collections.Concurrent;

namespace Infrastructure.Mediator.Context
{
    public class Context
    {
        public static Context Current => ContextStore.CurrentContext;

        public ushort TraceId { get; }
        public ushort ParentId { get; }
        public ushort CurrentId { get; }

        internal ConcurrentDictionary<string, object> Store { get; }

        internal Context(ushort traceId, ushort parentId, ushort currentId, ConcurrentDictionary<string, object> store)
        {
            TraceId = traceId;
            ParentId = parentId;
            CurrentId = currentId;
            Store = store;
        }

        public T FromStore<T>(string key, Func<T> defaultFactory = null)
        {
            var result = defaultFactory == null
                ? (Store.TryGetValue(key, out var val) ? val : default)
                : Store.GetOrAdd(key, k => defaultFactory());
            return (T)result;
        }

        public string Log()
        {
            return $"TraceId:{TraceId} ParentId:{ParentId} CurrentId:{CurrentId}";
        }
    }
}
