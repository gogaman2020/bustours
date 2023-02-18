using Infrastructure.Common.Logging;
using Infrastructure.Mediator.Context;
using Infrastructure.Mediator.LockManager;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Mediator
{
    public abstract class MediatorBase: IMediator
    {
        private readonly IContextFactory _contextFactory;
        private readonly ILockManager _lockManager;
        private static readonly ILogger _logger = Log.For<MediatorBase>();

        protected MediatorBase(IContextFactory contextFactory, ILockManager lockManager)
        {
            _contextFactory = contextFactory;
            _lockManager = lockManager;
        }

        public async Task RunCommandAsync(IMediatorCommand mediatorCommand, Func<Task> beforeRollback = null)
        {
            var context = Context.Context.Current;
            if (context == null)
            {
                var toRollback = mediatorCommand.BeforeRollback != null;

                await NotifyCommand(mediatorCommand);

                await _contextFactory.UseContextAsync(() => BeginCommandAsync(mediatorCommand));
            }
            else
            {
                LogQueuedCommand(mediatorCommand);
                AddToQueue(mediatorCommand);
            }
        }

        public async Task<TResult> RunCommandAsync<T, TResult>(IMediatorCommand<T, TResult> mediatorCommand, Func<TResult, Task> beforeRollback = null)
            where TResult : MediatorCommandResult<T>
        {
            if (mediatorCommand.IsQuery)
            {
                return await RunQueryAsync(mediatorCommand);
            }
            else
            {
                var context = Context.Context.Current;
                if (context == null)
                {
                    TResult result = null;
                    await _contextFactory.UseContextAsync(async () => result = await BeginCommandAsync(mediatorCommand, beforeRollback));
                    return result;
                }
                else
                {
                    throw new NotImplementedException("Don't use query from query");
                }
            }
        }

        public async Task<TResult> RunQueryAsync<T, TResult>(IMediatorCommand<T, TResult> mediatorQuery)
            where TResult : MediatorCommandResult<T>
        {
            TResult result = null;
            await _contextFactory.UseContextAsync(async () => result = await RunCommandInnerAsync(mediatorQuery));
            return result;
        }

        protected virtual async Task BeginCommandAsync(IMediatorCommand mediatorCommand, Func<Task> beforeRollback = null)
        {
            SetQueue();

            await _lockManager.WithLockAsync(mediatorCommand.Key(), () => RunCommandInnerAsync(mediatorCommand));

            await RunCommandLoopAsync(GetQueue());

            if (mediatorCommand.BeforeRollback != null)
            {
                await mediatorCommand.BeforeRollback(mediatorCommand);
                await OnRollback();
            }
            else if (beforeRollback != null)
            {
                await beforeRollback();
                await OnRollback();
            }
            else
            {
                await NotifyCommand(mediatorCommand);
                await OnComplete();
            }
        }

        protected virtual async Task<TResult> BeginCommandAsync<T, TResult>(IMediatorCommand<T, TResult> mediatorCommand, Func<TResult, Task> beforeRollback = null)
            where TResult : MediatorCommandResult<T>
        {
            TResult result = null;
            await _lockManager.WithLockAsync(mediatorCommand.Key(), async () => result = await RunCommandInnerAsync(mediatorCommand));
            if (beforeRollback != null)
            {
                await beforeRollback(result);
                await OnRollback();
            }
            else if (await RollBackOnResult(result))
            {
                await OnRollback();
            }
            else
            {
                await OnComplete();
            }
            return result;
        }

        protected virtual Task<bool> RollBackOnResult<T>(MediatorCommandResult<T> result)
        {
            return Task.FromResult(false);
        }

        protected virtual Task OnRollback()
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnComplete()
        {
            return Task.CompletedTask;
        }

        private async Task RunCommandLoopAsync(ConcurrentQueue<IMediatorCommand> queue)
        {
            if (queue == null)
            {
                return;
            }

            while (queue.TryDequeue(out var mediatorCommand))
            {
                //subcontext
                await _contextFactory.UseContextAsync(async () =>
                {
                    SetQueue();
                    await _lockManager.WithLockAsync(mediatorCommand.Key(),
                        () => BeginCommandAsync(mediatorCommand));
                    await RunCommandLoopAsync(GetQueue());
                });
            }
        }

        private static readonly AsyncLocal<Lazy<ContextQueue>> LocalQueue = new AsyncLocal<Lazy<ContextQueue>>();

        private void SetQueue()
        {
            LocalQueue.Value = new Lazy<ContextQueue>(() => new ContextQueue());
        }

        private void AddToQueue(IMediatorCommand mediatorCommand)
        {
            LocalQueue.Value.Value.Queue.Enqueue(mediatorCommand);
        }

        private ConcurrentQueue<IMediatorCommand> GetQueue()
        {
            var lazy = LocalQueue.Value;
            return lazy.IsValueCreated
                ? lazy.Value.Queue
                : null;
        }

        private static async Task RunCommandInnerAsync(IMediatorCommand mediatorCommand)
        {
            LogCommand(mediatorCommand);
            try
            {
                await mediatorCommand.ExecuteAsync();
            }
            catch (Exception e)
            {
                LogError(mediatorCommand, e);
                throw;
            }
            finally
            {
                LogCommandComplete(mediatorCommand);
            }
        }

        private static async Task<TResult> RunCommandInnerAsync<T, TResult>(IMediatorCommand<T, TResult> mediatorCommand)
            where TResult : MediatorCommandResult<T>
        {
            LogCommand(mediatorCommand);
            try
            {
                return await mediatorCommand.ExecuteAsync();
            }
            catch (Exception e)
            {
                LogError(mediatorCommand, e);
                throw;
            }
            finally
            {
                LogCommandComplete(mediatorCommand);
            }
        }

        private static void LogCommand(IMediatorIdentity mediatorCommand)
        {
            _logger.LogInformation(
                $"Begin command {mediatorCommand.GetType().Name}:{mediatorCommand.Log()} in context {Context.Context.Current.Log()}");
        }

        private static void LogCommandComplete(IMediatorIdentity mediatorCommand)
        {
            _logger.LogInformation(
                $"End command {mediatorCommand.GetType().Name} in context {Context.Context.Current.Log()}");
        }

        private static void LogError(IMediatorIdentity mediatorCommand, Exception e)
        {
            _logger.LogError(e,
                $"Error in command {mediatorCommand.GetType().Name}:{mediatorCommand.Log()} in context {Context.Context.Current.Log()}");
        }

        private static void LogQueuedCommand(IMediatorIdentity mediatorCommand)
        {
            _logger.LogInformation(
                $"Queued {mediatorCommand.GetType().Name}:{mediatorCommand.Log()} in context {Context.Context.Current.Log()}");
        }

        private static Task NotifyCommand(IMediatorCommand mediatorCommand)
        {
            return mediatorCommand.NotifyAsync();
        }
    }
}
