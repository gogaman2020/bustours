namespace Infrastructure.Db.Common
{
    public class SqlQueryObject : IQueryObject
    {
        private readonly string _sql;
        private readonly object _params;

        public SqlQueryObject(string sql, object @params = null)
        {
            _sql = sql;
            _params = @params;
        }

        public string GetQuery() => _sql;

        public object GetParams() => _params;
    }
}