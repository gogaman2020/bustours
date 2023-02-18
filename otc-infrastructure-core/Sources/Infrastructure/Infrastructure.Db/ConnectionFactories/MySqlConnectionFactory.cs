using System;
using System.Data.Common;
using Infrastructure.Common.DI;
using MySqlConnector;

namespace Infrastructure.Db.ConnectionFactories
{
    [InjectAsSingleton(typeof(MySqlConnectionFactory))]
    public sealed class MySqlConnectionFactory : BaseConnectionFactory
    {
        private const int DeadlockErrorNumber = 1213;
        private const int LockingErrorNumber = 1205;
        //private const int UpdateConflictErrorNumber = 3960;

        protected override DbConnection GetConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }

        protected override bool IsDeadLock(DbException exception)
        {
            var mySqlEx = exception as MySqlException;
            return mySqlEx == null
                ? false
                : mySqlEx.Number == DeadlockErrorNumber ||
                  mySqlEx.Number == LockingErrorNumber /*||
                  mySqlEx.Number == UpdateConflictErrorNumber*/;
        }
    }
}
