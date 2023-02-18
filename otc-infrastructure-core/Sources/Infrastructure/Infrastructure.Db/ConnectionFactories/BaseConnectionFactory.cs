using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Common.Configs;
using Infrastructure.Common.Logging;
using Infrastructure.Db.Common;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Db.ConnectionFactories
{
    public abstract class BaseConnectionFactory: IConnectionFactory
    {
        private const int DefaultRetryCount = 10;
        private const int DelayMilliseconds = 500;
        private static readonly Dictionary<string, SemaphoreSlim> Locks = new Dictionary<string, SemaphoreSlim>();

        public async Task UseConnectionAsync(Func<IDbConnection, CancellationToken, Task> func,
            CancellationToken ct = default)
        {
            var config = Config.Get<DbConfig>();
            await using var connection = GetConnection(config.ConnectionString);
            await connection.OpenAsync(ct);
            await func(connection, ct);
        }

        public Task UseConnectionWithRetryAsync(Func<IDbConnection, CancellationToken, Task> func,
            CancellationToken ct = default, string keyLock = null)
        {
            return UseConnectionWithRetryInnerAsync(func, ct);
        }

        protected abstract DbConnection GetConnection(string connectionString);

        protected abstract bool IsDeadLock(DbException exception);

        //https://habr.com/company/mindbox/blog/261661/
        private async Task UseConnectionWithRetryInnerAsync(Func<IDbConnection, CancellationToken, Task> func,
            CancellationToken ct = default,
            string keyLock = null,
            int retryCount = DefaultRetryCount)
        {
            var attemptNumber = 0;
            var requestLock = GetLock(keyLock);
            while (true)
            {
                try
                {
                    if (requestLock != null) await requestLock.WaitAsync(ct);

                    try
                    {
                        await UseConnectionAsync(func, ct);
                        break;
                    }
                    catch (DbException exception)
                    {
                        if (!IsDeadLock(exception))
                        {
                            throw;
                        }
                        else if (attemptNumber > retryCount)
                        {
                            Log.For<BaseConnectionFactory>().LogWarning($"DeadLock was detected. Retry request falling. Key {keyLock}.");
                            throw;
                        }
                        else
                        {
                            Log.For<BaseConnectionFactory>().LogWarning($"DeadLock was detected. Retry request. Key {keyLock}. Attempt number {attemptNumber+1}");
                            await Task.Delay(DelayMilliseconds * (attemptNumber + 1), ct);
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                finally
                {
                    requestLock?.Release();
                }

                attemptNumber++;
            }

            if (requestLock != null)
            {
                GcLocks(requestLock);
            }
        }

        private static SemaphoreSlim GetLock(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;

            lock (Locks)
            {
                if (Locks.ContainsKey(key))
                {
                    return Locks[key];
                }
                else
                {
                    var semaphore = new SemaphoreSlim(1);
                    Locks.Add(key, semaphore);
                    return semaphore;
                }
            }
        }

        private void GcLocks(SemaphoreSlim requestLock)
        {
            if (requestLock.CurrentCount <= 0) return;

            lock (Locks)
            {
                var keys = Locks.Keys.Select(a => a).ToArray();

                foreach (var key in keys)
                {
                    var ss = Locks[key];
                    if (ss.CurrentCount >= 1)
                    {
                        ss.Dispose();
                        Locks.Remove(key);
                    }
                }
            }
        }

    }
}
