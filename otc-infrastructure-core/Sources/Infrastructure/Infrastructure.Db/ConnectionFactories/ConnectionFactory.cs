using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Common.Configs;
using Infrastructure.Db.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Db.ConnectionFactories
{
    public static class ConnectionFactoryDiFactory
    {
        public class DummyConnectionFactory : IConnectionFactory
        {
            public Task UseConnectionAsync(Func<IDbConnection, CancellationToken, Task> func,
                CancellationToken ct = default(CancellationToken))
            {
                return func(null, ct);
            }

            public Task UseConnectionWithRetryAsync(Func<IDbConnection, CancellationToken, Task> func,
               CancellationToken ct = default(CancellationToken), string keyLock = null)
            {
                return func(null, ct);
            }
        }

        private static readonly DummyConnectionFactory _dummyConnectionFactory = new DummyConnectionFactory();

        public static IConnectionFactory GetFactory(IServiceProvider sp)
        {
            var config = Config.Get<DbConfig>();
            switch (config.Type)
            {
                case DbConnectionFactoryType.PostgresSql:
                    return sp.GetRequiredService<PostgreSqlConnectionFactory>();
                case DbConnectionFactoryType.MsSql:
                    return sp.GetRequiredService<MsSqlConnectionFactory>();
                case DbConnectionFactoryType.MySql:
                    return sp.GetRequiredService<MySqlConnectionFactory>();
            }

            return _dummyConnectionFactory;
        }

        public static IConnectionFormatter GetFormatter(IServiceProvider sp)
        {
            var config = Config.Get<DbConfig>();
            switch (config.Type)
            {
                case DbConnectionFactoryType.PostgresSql:
                    return sp.GetRequiredService<PostgreSqlFormatter>();
                case DbConnectionFactoryType.MsSql:
                    return sp.GetRequiredService<MsSqlFormatter>();
                case DbConnectionFactoryType.MySql:
                    return sp.GetRequiredService<MySqlFormatter>();
            }

            return null;
        }
    }
}