using System;
using System.Threading.Tasks;

namespace Infrastructure.Mediator
{
    public interface IMediatorCommand: IMediatorIdentity
    {
        public Func<IMediatorCommand, Task> BeforeRollback => null;
        Task ExecuteAsync();
        Task NotifyAsync();
    }
    public interface IMediatorCommand<T, TResult> : IMediatorIdentity
        where TResult : MediatorCommandResult<T>
    {
        public Func<IMediatorCommand, Task> BeforeRollback => null;
        Task<TResult> ExecuteAsync();
        public virtual bool IsQuery => false;
    }
}
