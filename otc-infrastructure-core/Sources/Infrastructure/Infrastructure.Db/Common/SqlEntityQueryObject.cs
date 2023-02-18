namespace Infrastructure.Db.Common
{
    public class SqlEntityQueryObject<TEntity> : SqlEntityQueryObject<int, TEntity>
        where TEntity : IEntity
    {
        public SqlEntityQueryObject(TEntity entity, string sql) : base(entity, sql)
        {
        }
    }

    public class SqlEntityQueryObject<TId, TEntity> : BaseQueryObject<TId, TEntity>
        where TEntity : IEntity<TId>
    {
        private readonly string _sql;

        public SqlEntityQueryObject(TEntity entity, string sql) : base(entity)
        {
            _sql = sql;
        }

        public override string GetQuery() => _sql;
    }
}