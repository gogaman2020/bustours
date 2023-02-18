using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Common.DI;
using Infrastructure.Db.Common.Transactions;

namespace Infrastructure.Db.Common
{
    [InjectAsSingleton]
    public class BaseDb : IDb
    {
        private readonly IConnectionFactory _factory;
        private readonly ITransactionFactory _transactionFactory;

        public BaseDb(IConnectionFactory factory, ITransactionFactory transactionFactory)
        {
            _factory = factory;
            _transactionFactory = transactionFactory;
        }

        private async Task ExecuteAsync(IQueryObject[] queryObjects, int count, IDbCommands cmd, CancellationToken ct)
        {
            for (int i = 0; i < count; i++)
            {
                if (ct.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }

                var query = queryObjects[i];
                await cmd.ExecuteAsync(query);
            }
        }
        
        public Task ExecuteAsync(IQueryObject[] queryObjects, bool useTransaction = false,
            DbCommandSettings settings = null, CancellationToken ct = default(CancellationToken))
        {
            return WithConnectionAsync<object>(async (cmd, ctx) =>
            {
                await ExecuteAsync(queryObjects, queryObjects.Length, cmd, ctx);
                return null;
            }, useTransaction, settings, ct);
        }

        public Task<IEnumerable<T>> QueryAsync<T>(IQueryObject[] queryObjects, bool useTransaction = false,
            DbCommandSettings settings = null, CancellationToken ct = default(CancellationToken))
        {
            return WithConnectionAsync(async (cmd, ctx) =>
            {
                await ExecuteAsync(queryObjects, queryObjects.Length - 1, cmd, ctx);

                if (ctx.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }

                var queryLast = queryObjects[queryObjects.Length - 1];
                return await cmd.QueryAsync<T>(queryLast);
            }, useTransaction, settings, ct);
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(IQueryObject[] queryObjects, bool useTransaction = false,
            DbCommandSettings settings = null, CancellationToken ct = default(CancellationToken))
        {
            return WithConnectionAsync(async (cmd, ctx) =>
            {
                await ExecuteAsync(queryObjects, queryObjects.Length - 1, cmd, ctx);

                if (ctx.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }

                var queryLast = queryObjects[queryObjects.Length - 1];
                return await cmd.QueryFirstOrDefaultAsync<T>(queryLast);
            }, useTransaction, settings, ct);
        }

        public Task<T> QuerySingleOrDefaultAsync<T>(IQueryObject[] queryObjects, bool useTransaction = false,
            DbCommandSettings settings = null, CancellationToken ct = default(CancellationToken))
        {
            return WithConnectionAsync(async (cmd, ctx) =>
            {
                await ExecuteAsync(queryObjects, queryObjects.Length - 1, cmd, ctx);

                if (ctx.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }

                var queryLast = queryObjects[queryObjects.Length - 1];
                return await cmd.QuerySingleOrDefaultAsync<T>(queryLast);
            }, useTransaction, settings, ct);
        }
        
        public Task ExecuteAsync(Func<IDbCommands, CancellationToken, Task> command, bool useTransaction = false,
            DbCommandSettings settings = null, CancellationToken ct = default(CancellationToken))
        {
            return WithConnectionAsync<object>(async (db, ctx) =>
            {
                await command(db, ctx);
                return null;
            }, useTransaction, settings, ct);
        }

        public Task<T> QueryOneAsync<T>(Func<IDbCommands, CancellationToken, Task<T>> command,
            bool useTransaction = false, DbCommandSettings settings = null, CancellationToken ct = default(CancellationToken))
        {
            return WithConnectionAsync(command, useTransaction, settings, ct);
        }

        private async Task<T> WithConnectionAsync<T>(Func<IDbCommands, CancellationToken, Task<T>> func,
            bool useTransaction, DbCommandSettings settings, CancellationToken ct)
        {
            T result = default;
            //есть текущая транзакция
            var txCurrent = _transactionFactory.Current;
            if (txCurrent != null)
            {
                await txCurrent.RunAsync(async (commands, ctx) =>
                {
                    result = await func(commands, ctx);
                }, ct, settings);
                return result;
            }

            await _factory.UseConnectionAsync(async (connection, ct) =>
            {
                if (useTransaction)
                {
                    using var tx = connection.BeginTransaction();
                    result = await RunCommandsAsync(connection, tx);
                    tx.Commit();
                }
                else
                {
                    result = await RunCommandsAsync(connection);
                }

            }, ct);

            return result;
            
            async Task<T> RunCommandsAsync(IDbConnection connection, IDbTransaction tx = null)
            {
                var commands = new BaseDbCommands(connection, tx, settings);
                return await func(commands, ct);
            }
        }

        public Task<T[]> QueryAsync<T>(Func<IDbCommands, CancellationToken, Task<T[]>> command,
            bool useTransaction = false, DbCommandSettings settings = null, 
            CancellationToken ct = default(CancellationToken))
        {
            return WithConnectionAsync(command, useTransaction, settings, ct);
        }

        public Task ExecuteAsync(IQueryObject queryObject, DbCommandSettings settings = null)
        {
            return WithConnectionAsync<object>(async (cmd, ct) =>
            {
                await cmd.ExecuteAsync(queryObject);
                return null;
            }, false, settings, default(CancellationToken));
        }

        public Task<IEnumerable<T>> QueryAsync<T>(IQueryObject queryObject, DbCommandSettings settings = null)
        {
            return WithConnectionAsync((cmd, ct) => cmd.QueryAsync<T>(queryObject), false, settings, default(CancellationToken));
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(IQueryObject queryObject, DbCommandSettings settings = null)
        {
            return WithConnectionAsync((cmd, ct) => cmd.QueryFirstOrDefaultAsync<T>(queryObject), false, settings, default(CancellationToken));
        }
        
        public Task<T> QuerySingleOrDefaultAsync<T>(IQueryObject queryObject, DbCommandSettings settings = null)
        {
            return WithConnectionAsync((cmd, ct) => cmd.QuerySingleOrDefaultAsync<T>(queryObject), false, settings, default(CancellationToken));
        }

        public Task<T> ExecuteFuncAsync<T>(Func<IDbConnection, string, object, IDbTransaction, int?, Task<T>> func, IQueryObject queryObject, DbCommandSettings settings = null)
        {
            return WithConnectionAsync((cmd, ct) => cmd.ExecuteFuncAsync<T>(func, queryObject), false, settings, default(CancellationToken));
        }

        public Task<T[]> QueryAsyncx<T,T1,T2,T3>(Func<IDbCommands, CancellationToken, Task<T[]>> command,
    bool useTransaction = false, DbCommandSettings settings = null, CancellationToken ct = default(CancellationToken))
        {
            return WithConnectionAsync(command, useTransaction, settings, ct);
        }

        public Task<IEnumerable<T>> QueryAsync<T, T1>(IQueryObject queryObject, Func<T, T1, T> map, DbCommandSettings settings = null)
        {
            return WithConnectionAsync((cmd, ct) => cmd.QueryAsync(queryObject, map), false, settings, default(CancellationToken));
        }

        public Task<IEnumerable<T>> QueryAsync<T, T1, T2>(IQueryObject queryObject, Func<T, T1, T2, T> map, DbCommandSettings settings = null)
        {
            return WithConnectionAsync((cmd, ct) => cmd.QueryAsync(queryObject, map), false, settings, default(CancellationToken));
        }

        public Task<IEnumerable<T>> QueryAsync<T, T1, T2, T3>(IQueryObject queryObject, Func<T, T1, T2, T3, T> map, DbCommandSettings settings = null)
        {
            return WithConnectionAsync((cmd, ct) => cmd.QueryAsync(queryObject, map), false, settings, default(CancellationToken));
        }

        public Task<IEnumerable<T>> QueryAsync<T, T1, T2, T3, T4>(IQueryObject queryObject, Func<T, T1, T2, T3, T4, T> map, DbCommandSettings settings = null)
        {
            return WithConnectionAsync((cmd, ct) => cmd.QueryAsync(queryObject, map), false, settings, default(CancellationToken));
        }

        public Task<IEnumerable<T>> QueryAsync<T, T1, T2, T3, T4, T5>(IQueryObject queryObject, Func<T, T1, T2, T3, T4, T5, T> map, DbCommandSettings settings = null)
        {
            return WithConnectionAsync((cmd, ct) => cmd.QueryAsync(queryObject, map), false, settings, default(CancellationToken));
        }

        public Task<IEnumerable<T>> QueryAsync<T, T1, T2, T3, T4, T5, T6>(IQueryObject queryObject, Func<T, T1, T2, T3, T4, T5, T6, T> map, DbCommandSettings settings = null)
        {
            return WithConnectionAsync((cmd, ct) => cmd.QueryAsync(queryObject, map), false, settings, default(CancellationToken));
        }
    }
}