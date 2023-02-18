using System;
using System.Threading.Tasks;

namespace Infrastructure.Mediator
{
    public interface IMediator
    {
        Task RunCommandAsync(IMediatorCommand mediatorCommand, Func<Task> beforeRollback = null);

        Task<TResult> RunCommandAsync<T, TResult>(IMediatorCommand<T, TResult> mediatorCommand, Func<TResult, Task> beforeRollback = null)
            where TResult : MediatorCommandResult<T>;

        public Task<TResult> RunQueryAsync<T, TResult>(IMediatorCommand<T, TResult> mediatorQuery)
            where TResult : MediatorCommandResult<T>
        {
            return RunCommandAsync(mediatorQuery);
        }
    }
}
