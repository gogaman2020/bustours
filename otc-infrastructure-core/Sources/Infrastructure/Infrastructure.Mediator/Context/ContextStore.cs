using System.Threading;

namespace Infrastructure.Mediator.Context
{
    internal static class ContextStore
    {
        public static readonly AsyncLocal<Context> _store = new AsyncLocal<Context>();
        public static Context CurrentContext => _store.Value;

        public static void SetContext(Context context)
        {
            _store.Value = context;
        }
    }
}
