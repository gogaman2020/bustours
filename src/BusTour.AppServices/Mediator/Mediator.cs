using Infrastructure.Common.DI;
using Infrastructure.Db.Common.Transactions;
using Infrastructure.Mediator;
using Infrastructure.Mediator.Context;
using Infrastructure.Mediator.LockManager;
using System;
using System.Data;
using System.Threading.Tasks;

namespace BusTour.AppServices.Mediator
{
    [InjectAsSingleton]
    public class Mediator: MediatorBase
    {
        private readonly ITransactionFactory _transactionFactory;

        public Mediator(IContextFactory contextFactory, ILockManager lockManager, ITransactionFactory transactionFactory)
            :base(contextFactory, lockManager)
        {
            _transactionFactory = transactionFactory;
        }

        protected override async Task BeginCommandAsync(IMediatorCommand mediatorCommand, Func<Task> beforeRollback = null)
        {
            var toRollback = beforeRollback != null;
            await _transactionFactory.UseTransactionWithRetryAsync(
                (tr) => base.BeginCommandAsync(mediatorCommand, beforeRollback),
                toRollback ? IsolationLevel.ReadCommitted : IsolationLevel.Serializable,
                nameof(Mediator));
        }
        protected override async Task<TResult> BeginCommandAsync<T, TResult>(IMediatorCommand<T, TResult> mediatorCommand, Func<TResult, Task> beforeRollback = null)
        {
            var toRollback = beforeRollback != null;
            return await _transactionFactory.UseTransactionWithRetryAsync(
                (tr) => base.BeginCommandAsync(mediatorCommand, beforeRollback),
                toRollback ? IsolationLevel.ReadCommitted : IsolationLevel.Serializable,
                nameof(Mediator));
        }

        protected override Task<bool> RollBackOnResult<T>(MediatorCommandResult<T> result)
        {
            return Task.FromResult(!string.IsNullOrEmpty(result.ErrorMessage) || result.ErrorData != null);
        }

        protected override Task OnComplete()
        {
            _transactionFactory.Current?.Commit();
            return base.OnComplete();
        }

        protected override Task OnRollback()
        {
            _transactionFactory.Current?.Rollback();
            return base.OnRollback();
        }
    }
}
