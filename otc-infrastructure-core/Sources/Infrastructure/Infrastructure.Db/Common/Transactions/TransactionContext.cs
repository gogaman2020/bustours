using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Db.Common.Transactions
{
    public class TransactionContext : ITransactionContext, ITransaction
    {
        private volatile Task _task = Task.CompletedTask;
        private static readonly AsyncLocal<Task> _taskLocal = new AsyncLocal<Task>();

        public IDbConnection Connection { get; }
        public IDbTransaction Transaction { get; private set; }

        public TransactionContext(IDbConnection connection, IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            Connection = connection;
            //todo isolation level
            //also it can be begin transaction in sql command
            Transaction = Connection?.BeginTransaction(isolationLevel);
        }

        public void Rollback()
        {
            CompleteTx(tx => tx.Rollback());
        }

        public void Commit()
        {
            CompleteTx(tx => tx.Commit());
        }

        public Task RunAsync(Func<IDbCommands, CancellationToken, Task> func, CancellationToken ct = default,
            DbCommandSettings settings = null)
        {
            var tx = Transaction;
            if (tx == null)
            {
                throw new InvalidOperationException("Transaction is closed");
            }

            Func<Task, Task> newTask = async task =>
            {
                var noParent = _taskLocal.Value == null;
                if (noParent)
                {
                    try
                    {
                        await task;
                    }
                    catch (InvalidOperationException)
                    {
                        throw;
                    }
                    catch
                    {
                    }
                }

                if (tx != Transaction)
                {
                    throw new InvalidOperationException();
                }

                try
                {
                    if (noParent)
                    {
                        //set parent
                        _taskLocal.Value = _task;
                    }

                    var commands = new BaseDbCommands(Connection, Transaction, settings);
                    await func(commands, ct);
                }
                finally
                {
                    if (noParent)
                    {
                        //clear parent
                        _taskLocal.Value = null;
                    }
                }
            };

            lock (tx)
            {
                _task = newTask(_task);
                return _task;
            }
        }

        private void CompleteTx(Action<IDbTransaction> action)
        {
            if (Connection == null)
            {
                return;
            }
            
            var tx = Transaction;
            if (tx == null)
            {
                throw new InvalidOperationException();
            }

            lock (tx)
            {
                action(tx);
                if (tx == Transaction)
                {
                    Transaction = null;
                }

                Connection.Close();
            }
        }

        public void Dispose()
        {
            //_task?.GetAwaiter().GetResult();
            if (Transaction != null)
            {
                Rollback();
                Transaction = null;
            }

            Connection?.Dispose();
        }
    }
}