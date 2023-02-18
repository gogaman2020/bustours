using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Db.Common
{
    public interface IDb : IDbCommands
    {
        #region Группы запросов

        Task ExecuteAsync(IQueryObject[] queryObjects, bool useTransaction = false, DbCommandSettings settings = null,
            CancellationToken ct = default(CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryObjects"></param>
        /// <param name="useTransaction"></param>
        /// <param name="ct"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>Результат последнего запроса в списке</returns>
        Task<IEnumerable<T>> QueryAsync<T>(IQueryObject[] queryObjects, bool useTransaction = false,
            DbCommandSettings settings = null, CancellationToken ct = default(CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryObjects"></param>
        /// <param name="useTransaction"></param>
        /// <param name="ct"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>Результат последнего запроса в списке</returns>
        Task<T> QueryFirstOrDefaultAsync<T>(IQueryObject[] queryObjects, bool useTransaction = false,
            DbCommandSettings settings = null, CancellationToken ct = default(CancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryObjects"></param>
        /// <param name="useTransaction"></param>
        /// <param name="ct"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>Результат последнего запроса в списке</returns>
        Task<T> QuerySingleOrDefaultAsync<T>(IQueryObject[] queryObjects, bool useTransaction = false,
            DbCommandSettings settings = null, CancellationToken ct = default(CancellationToken));

        #endregion

        #region Команды 

        Task ExecuteAsync(Func<IDbCommands, CancellationToken, Task> command, bool useTransaction = false,
            DbCommandSettings settings = null, CancellationToken ct = default(CancellationToken));

        Task<T> QueryOneAsync<T>(Func<IDbCommands, CancellationToken, Task<T>> command, bool useTransaction = false, 
            DbCommandSettings settings = null, CancellationToken ct = default(CancellationToken));

        Task<T[]> QueryAsync<T>(Func<IDbCommands, CancellationToken, Task<T[]>> command, bool useTransaction = false, 
            DbCommandSettings settings = null, CancellationToken ct = default(CancellationToken));

        #endregion
    }
}