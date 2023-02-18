using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace Infrastructure.Db.Common
{
    public struct BaseDbCommands : IDbCommands
    {
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;
        private readonly DbCommandSettings _settings;

        public BaseDbCommands(IDbConnection connection, IDbTransaction transaction, DbCommandSettings settings = null)
        {
            _connection = connection;
            _transaction = transaction;
            _settings = settings;
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        private int? GetTimeout(DbCommandSettings settings)
        {
            return (settings ?? _settings)?.Timeout;
        }

        private string GetSplitOn(DbCommandSettings settings)
        {
            return (settings ?? _settings)?.SplitOn ?? "Id";
        }

        public async Task ExecuteAsync(IQueryObject queryObject, DbCommandSettings settings = null)
        {
            await _connection.ExecuteAsync(queryObject.GetQuery(), queryObject.GetParams(),
                _transaction, GetTimeout(settings));
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(IQueryObject queryObject, DbCommandSettings settings = null)
        {
            return await _connection.QueryAsync<T>(queryObject.GetQuery(), queryObject.GetParams(),
                _transaction, GetTimeout(settings));
        }

        public async Task<IEnumerable<T>> QueryAsync<T,T1>(IQueryObject queryObject,Func<T, T1, T> map, DbCommandSettings settings = null)
        {
            return await _connection.QueryAsync<T,T1,T>(queryObject.GetQuery(), map, queryObject.GetParams(),_transaction, commandTimeout: GetTimeout(settings), splitOn: GetSplitOn(settings));
        }

        public async Task<IEnumerable<T>> QueryAsync<T, T1, T2>(IQueryObject queryObject, Func<T, T1, T2, T> map, DbCommandSettings settings = null)
        {
            return await _connection.QueryAsync<T, T1,T2, T>(queryObject.GetQuery(), map, queryObject.GetParams(), _transaction, commandTimeout: GetTimeout(settings), splitOn: GetSplitOn(settings));
        }

        public async Task<IEnumerable<T>> QueryAsync<T, T1, T2, T3>(IQueryObject queryObject, Func<T, T1, T2, T3, T> map, DbCommandSettings settings = null)
        {
            return await _connection.QueryAsync<T, T1, T2, T3, T>(queryObject.GetQuery(), map, queryObject.GetParams(), _transaction, commandTimeout: GetTimeout(settings), splitOn: GetSplitOn(settings));
        }

        public async Task<IEnumerable<T>> QueryAsync<T, T1, T2, T3, T4>(IQueryObject queryObject, Func<T, T1, T2, T3, T4, T> map, DbCommandSettings settings = null)
        {
            return await _connection.QueryAsync<T, T1, T2, T3, T4, T>(queryObject.GetQuery(), map, queryObject.GetParams(), _transaction, commandTimeout: GetTimeout(settings), splitOn: GetSplitOn(settings));
        }

        public async Task<IEnumerable<T>> QueryAsync<T, T1, T2, T3, T4, T5>(IQueryObject queryObject, Func<T, T1, T2, T3, T4, T5, T> map, DbCommandSettings settings = null)
        {
            return await _connection.QueryAsync<T, T1, T2, T3, T4, T5, T>(queryObject.GetQuery(), map, queryObject.GetParams(), _transaction, commandTimeout: GetTimeout(settings), splitOn: GetSplitOn(settings));
        }

        public async Task<IEnumerable<T>> QueryAsync<T, T1, T2, T3, T4, T5, T6>(IQueryObject queryObject, Func<T, T1, T2, T3, T4, T5, T6, T> map, DbCommandSettings settings = null)
        {
            return await _connection.QueryAsync<T, T1, T2, T3, T4, T5, T6, T>(queryObject.GetQuery(), map, queryObject.GetParams(), _transaction, commandTimeout: GetTimeout(settings), splitOn: GetSplitOn(settings));
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(IQueryObject queryObject, DbCommandSettings settings = null)
        {
            return await _connection.QueryFirstOrDefaultAsync<T>(queryObject.GetQuery(), queryObject.GetParams(),
                _transaction, GetTimeout(settings));
        }
        
        public async Task<T> QuerySingleOrDefaultAsync<T>(IQueryObject queryObject, DbCommandSettings settings = null)
        {
            return await _connection.QuerySingleOrDefaultAsync<T>(queryObject.GetQuery(), queryObject.GetParams(),
                _transaction, GetTimeout(settings));
        }

        public async Task<T> ExecuteFuncAsync<T>(Func<IDbConnection, string, object, IDbTransaction, int?, Task<T>> func, IQueryObject queryObject, DbCommandSettings settings = null)
        {
            return await func.Invoke(_connection, queryObject.GetQuery(), queryObject.GetParams(), _transaction, GetTimeout(settings));
        }

    }
}