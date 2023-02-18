using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Db.Common
{
    public interface IConnectionFactory
    {
        Task UseConnectionAsync(Func<IDbConnection, CancellationToken, Task> func,
            CancellationToken ct = default(CancellationToken));

        Task UseConnectionWithRetryAsync(Func<IDbConnection, CancellationToken, Task> func,
           CancellationToken ct = default(CancellationToken), string keyLock = null);
    }
}
