using Infrastructure.Common.DI;
using System.Threading.Tasks;

namespace Infrastructure.Mediator
{
    public abstract class HighLevelMediatorCommand : IMediatorCommand
    {
        protected HighLevelMediatorCommand(string id)
        {
            Id = id;
        }

        public IMediator Mediator => IoC.GetRequiredService<IMediator>();
        public virtual string Id { get; }
        public abstract Task ExecuteAsync();

        public string Log()
        {
            return this.Key();
        }

        public Task NotifyAsync()
        {
            return Task.CompletedTask;
        }
    }

    public abstract class HighLevelMediatorCommand<T> : IMediatorCommand<T, MediatorCommandResult<T>>
    {
        protected HighLevelMediatorCommand()
        {
        }

        protected IMediator Mediator => IoC.GetRequiredService<IMediator>();
        public virtual bool IsQuery => false;
        public virtual string Id { get; set; }
        public abstract Task<MediatorCommandResult<T>> ExecuteAsync();

        public string Log()
        {
            return this.Key();
        }

        public Task NotifyAsync()
        {
            return Task.CompletedTask;
        }

        protected MediatorCommandResult<T> Fail()
        {
            return MediatorCommandResult<T>.Fail();
        }

        protected MediatorCommandResult<T> Fail(string message, object data = null)
        {
            return MediatorCommandResult<T>.Fail(message, data);
        }

        protected MediatorCommandResult<T> Success(T result)
        {
            return MediatorCommandResult<T>.Success(result);
        }
    }

    public abstract class MediatorQuery<T> : HighLevelMediatorCommand<T>
    {
        public override bool IsQuery => true;
    }

    public abstract class HighLevelMediatorCommand<T, TE> : HighLevelMediatorCommand<T>
    {
        protected MediatorCommandResult<T> Fail(TE error)
        {
            return MediatorCommandResult<T>.Fail(string.Empty, error);
        }
    }
}
