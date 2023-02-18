using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Infrastructure.Db.Common
{
    public interface IDbCommands
    {
        Task ExecuteAsync(IQueryObject queryObject, DbCommandSettings settings = null);

        Task<IEnumerable<T>> QueryAsync<T>(IQueryObject queryObject, DbCommandSettings settings = null);

        Task<IEnumerable<T>> QueryAsync<T, T1>(IQueryObject queryObject, Func<T, T1, T> map, DbCommandSettings settings = null);

        Task<IEnumerable<T>> QueryAsync<T, T1, T2>(IQueryObject queryObject, Func<T, T1, T2, T> map, DbCommandSettings settings = null);

        Task<IEnumerable<T>> QueryAsync<T, T1, T2, T3>(IQueryObject queryObject, Func<T, T1, T2, T3, T> map, DbCommandSettings settings = null);

        Task<IEnumerable<T>> QueryAsync<T, T1, T2, T3, T4>(IQueryObject queryObject, Func<T, T1, T2, T3, T4, T> map, DbCommandSettings settings = null);

        Task<IEnumerable<T>> QueryAsync<T, T1, T2, T3, T4, T5>(IQueryObject queryObject, Func<T, T1, T2, T3, T4, T5, T> map, DbCommandSettings settings = null);

        Task<IEnumerable<T>> QueryAsync<T, T1, T2, T3, T4, T5, T6>(IQueryObject queryObject, Func<T, T1, T2, T3, T4, T5, T6, T> map, DbCommandSettings settings = null);

        Task<T> QueryFirstOrDefaultAsync<T>(IQueryObject queryObject, DbCommandSettings settings = null);
     
        Task<T> QuerySingleOrDefaultAsync<T>(IQueryObject queryObject, DbCommandSettings settings = null);

        Task<T> ExecuteFuncAsync<T>(Func<IDbConnection, string, object, IDbTransaction, int?, Task<T>> func, IQueryObject queryObject, DbCommandSettings settings = null);
    }
}