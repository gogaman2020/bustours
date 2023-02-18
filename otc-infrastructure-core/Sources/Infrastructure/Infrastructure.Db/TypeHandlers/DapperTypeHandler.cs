using Dapper;

namespace Infrastructure.Db.TypeHandlers
{
    [DapperTypeHandler]
    public abstract class DapperTypeHandler<T> : SqlMapper.TypeHandler<T>
    {
        
    }
}