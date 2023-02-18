using System.Data.Common;
using Infrastructure.Common.DI;
using Npgsql;

namespace Infrastructure.Db.ConnectionFactories
{
    [InjectAsSingleton(typeof(PostgreSqlConnectionFactory))]
    public sealed class PostgreSqlConnectionFactory : BaseConnectionFactory
    {
        private static readonly string _deadLockCode = "40P01";

        protected override DbConnection GetConnection(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }

        protected override bool IsDeadLock(DbException exception)
        {
            var pgEx = exception as PostgresException;
            return pgEx == null
                ? false
                : string.Equals(pgEx.SqlState, _deadLockCode);
        }
    }
}