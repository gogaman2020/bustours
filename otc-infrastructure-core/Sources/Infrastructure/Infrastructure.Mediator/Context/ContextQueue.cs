using System.Collections.Concurrent;

namespace Infrastructure.Mediator.Context
{
    public class ContextQueue
    {
        public ContextQueue()
        {
            Context = Context.Current;
            Queue = new ConcurrentQueue<IMediatorCommand>();
        }

        public Context Context { get; }
        public ConcurrentQueue<IMediatorCommand> Queue { get; }
    }
}
