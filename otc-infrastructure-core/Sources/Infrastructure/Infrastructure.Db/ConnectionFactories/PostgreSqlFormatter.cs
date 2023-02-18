using Infrastructure.Common.DI;
using Infrastructure.Db.Common;

namespace Infrastructure.Db.ConnectionFactories
{
    [InjectAsSingleton(typeof(PostgreSqlFormatter))]
    public class PostgreSqlFormatter : IConnectionFormatter
    {
        public string Table(string tableName)
        {
            return tableName?.ToLower();
        }

        public string Column(string columnName)
        {
            return columnName?.ToLower();
        }
    }
}