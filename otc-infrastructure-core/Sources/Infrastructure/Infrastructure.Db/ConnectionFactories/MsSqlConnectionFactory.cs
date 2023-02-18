using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Common.Configs;
using Infrastructure.Common.DI;
using Infrastructure.Db.Common;
using Microsoft.Extensions.Options;

namespace Infrastructure.Db.ConnectionFactories
{
    [InjectAsSingleton(typeof(MsSqlConnectionFactory))]
    public sealed class MsSqlConnectionFactory : BaseConnectionFactory
    {
        private static readonly int _deadLockNumber = 1205;
        private static readonly int _lockingNumber = 1222;
        private static readonly int _updateConflictNumber = 3960;

        protected override DbConnection GetConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        protected override bool IsDeadLock(DbException exception)
        {
            var sqlEx = exception as SqlException;
            return sqlEx == null
                ? false
                : sqlEx.Errors.Cast<SqlError>()
                    .Any(p =>
                        p.Number == _deadLockNumber ||
                        p.Number == _lockingNumber ||
                        p.Number == _updateConflictNumber);
        }
    }
}
