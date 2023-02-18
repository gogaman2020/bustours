using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Common.DI;

namespace Infrastructure.Db.Common.Transactions
{
    [InjectAsSingleton]
    public class TransactionFactory : ITransactionFactory
    {
        private static readonly AsyncLocal<TransactionContext> Context = new AsyncLocal<TransactionContext>();

        private readonly IConnectionFactory _connectionFactory;

        public TransactionFactory(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public ITransaction Current => Context.Value;

        public async Task UseTransactionAsync(Func<ITransaction, Task> func, IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            await _connectionFactory.UseConnectionAsync((connection, ct) =>
                UseNewContextAsync(connection, async context => await func(context), isolationLevel));
        }

        public async Task<T> UseTransactionAsync<T>(Func<ITransaction, Task<T>> func, IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            T result = default;
            await _connectionFactory.UseConnectionAsync(async (connection, ct) =>
            {
                result = await UseNewContextAsync<T>(connection, async context => await func(context), isolationLevel);
            });

            return result;
        }

        public async Task UseTransactionWithRetryAsync(Func<ITransaction, Task> func, IsolationLevel isolationLevel = IsolationLevel.Serializable, string keyLock = null)
        {
            await _connectionFactory.UseConnectionWithRetryAsync((connection, ct) =>
                UseNewContextAsync(connection, async context => await func(context), isolationLevel),
                keyLock: keyLock);
        }

        public async Task<T> UseTransactionWithRetryAsync<T>(Func<ITransaction, Task<T>> func, IsolationLevel isolationLevel = IsolationLevel.Serializable, string keyLock = null)
        {
            T result = default;
            await _connectionFactory.UseConnectionWithRetryAsync(async (connection, ct) =>
            {
                result = await UseNewContextAsync<T>(connection, async context => await func(context), isolationLevel);
            }, keyLock: keyLock);

            return result;
        }

        private async Task UseNewContextAsync(IDbConnection connection, Func<ITransaction, Task> func, IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            var context = new TransactionContext(connection, isolationLevel);
            Context.Value = context;
            try
            {
                await func(context);
            }
            finally
            {
                context.Dispose();
                Context.Value = null;
            }
        }

        private async Task<T> UseNewContextAsync<T>(IDbConnection connection, Func<ITransaction, Task<T>> func, IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            var context = new TransactionContext(connection, isolationLevel);
            Context.Value = context;
            try
            {
                return await func(context);
            }
            finally
            {
                context.Dispose();
                Context.Value = null;
            }
        }
    }
}