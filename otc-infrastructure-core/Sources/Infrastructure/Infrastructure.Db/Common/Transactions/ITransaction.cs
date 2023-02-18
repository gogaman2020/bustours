using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Db.Common.Transactions
{
    public interface ITransaction : IDisposable
    {
        void Rollback();
        void Commit();

        Task RunAsync(Func<IDbCommands, CancellationToken, Task> func, CancellationToken ct = default,
            DbCommandSettings settings = null);
    }
}