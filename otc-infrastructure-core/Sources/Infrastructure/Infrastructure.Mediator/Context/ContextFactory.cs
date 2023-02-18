using Infrastructure.Common.DI;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Infrastructure.Mediator.Context
{
    [InjectAsSingleton]
    public class ContextFactory : IContextFactory
    {
        private static ushort _seqIndex = 0;
        private static object _sync = new object();

        private readonly ILogger<ContextFactory> _logger;

        public ContextFactory(ILogger<ContextFactory> logger)
        {
            _logger = logger;
        }

        public async Task UseContextAsync(Func<Task> inContext)
        {
            var ctx = Context.Current;
            if (ctx == null)
            {
                await WithContextAsync(CreateContext(), inContext);
            }
            else
            {
                await WithContextAsync(CreateSubContext(ctx), inContext);
            }
        }

        public async Task UseContextAsync(Func<Task> inContext, Context context)
        {
            await WithContextAsync(context, inContext);
        }

        private static async Task WithContextAsync(Context ctx, Func<Task> inContext)
        {
            SetContext(ctx);
            await inContext();
        }

        // private static async Task ErrorLogWrapper(Func<Task> func)
        // {
        //     try
        //     {
        //         await func();
        //     }
        //     catch (Exception e)
        //     {
        //         _logger.LogError($"Error in context {Context.Current.Log()}", e);
        //     }
        // }

        private static void SetContext(Context ctx)
        {
            ContextStore.SetContext(ctx);
        }

        private static Context CreateContext()
        {
            return new Context(GetNextValue(), 0, 0, new ConcurrentDictionary<string, object>());
        }

        private static Context CreateSubContext(Context ctx)
        {
            return new Context(ctx.TraceId, ctx.CurrentId, GetNextValue(), ctx.Store);
        }

        private static ushort GetNextValue()
        {
            unchecked
            {
                return ++_seqIndex;
            }
        }
    }
}
