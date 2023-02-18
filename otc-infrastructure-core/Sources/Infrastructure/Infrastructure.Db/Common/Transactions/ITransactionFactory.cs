using System;
using System.Data;
using System.Threading.Tasks;

namespace Infrastructure.Db.Common.Transactions
{
    public interface ITransactionFactory
    {
        ITransaction Current { get; }
        Task UseTransactionAsync(Func<ITransaction, Task> func, IsolationLevel isolationLevel = IsolationLevel.Serializable);
        Task<T> UseTransactionAsync<T>(Func<ITransaction, Task<T>> func, IsolationLevel isolationLevel = IsolationLevel.Serializable);
        Task UseTransactionWithRetryAsync(Func<ITransaction, Task> func, IsolationLevel isolationLevel = IsolationLevel.Serializable, string keyLock = null);
        Task<T> UseTransactionWithRetryAsync<T>(Func<ITransaction, Task<T>> func, IsolationLevel isolationLevel = IsolationLevel.Serializable, string keyLock = null);
    }
}