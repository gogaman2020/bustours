using BusTour.Data.Repositories.Selections.Queries;
using BusTour.Domain.Entities;
using Dapper;
using Infrastructure.Common.DI;
using Infrastructure.Db.Common;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BusTour.Data.Repositories.Selections
{
    /// <summary>
    /// Репозиторий для запросов алгоритма подбора мест.
    /// </summary>
    [InjectAsSingleton]
    public class SelectionRepository : ISelectionRepository
    {
        private readonly ILogger _logger;
        private readonly IDb _db;

        /// <summary>
        /// Конструктор класса <see cref="SelectionRepository"/>
        /// </summary>
        /// <param name="context">Фабрика соединений с БД.</param>
        public SelectionRepository()
        {
            _logger = LogManager.GetCurrentClassLogger();
            _db = IoC.GetRequiredService<IDb>();
        }

        /// <inheritdoc/>
        public async Task<List<SelectionTable>> GetTourInfoAsync(int tourId)
        {
            try
            {
                return await _db.ExecuteFuncAsync(SelectTourInfoAsync, FilterQueryObject.For(new SelectionQuery { TourId = tourId }, SelectionQuery.SelectTourInfo));
            }
            catch (Exception e)
            {
                _logger.Error(e, "SelectionRepository.GetTourInfo threw error");
                throw;
            }
        }

        public async Task<List<ObjectPosition>> GetBusObjectsPositionsAsync(int tourId)
        {
            try
            {
                return await _db.ExecuteFuncAsync(SelectBusObjectsPositionsAsync, FilterQueryObject.For(new SelectionQuery { TourId = tourId }, SelectionQuery.SelectBusObjectsPositions));
            }
            catch (Exception e)
            {
                _logger.Error(e, "SelectionRepository.GetBusObjectsPositions threw error");
                throw;
            }
        }

        private async Task<List<SelectionTable>> SelectTourInfoAsync(IDbConnection connection, string sql, object param,
            IDbTransaction transaction, int? timeout)
        {
            var tables = new List<SelectionTable>();

            var queryResult = await connection
                .QueryAsync<SelectionTable, SelectionSeat, SelectionTable>(
                    sql,
                    (t, s) =>
                    {
                        var table = tables.Where(i => i.Id == t.Id).FirstOrDefault();

                        if (table == null)
                        {
                            table = t;
                            table.Seats = new List<SelectionSeat>();
                            tables.Add(table);
                        }

                        table.Seats.Add(s);

                        return t;
                    },
                    param,
                    transaction,
                    commandTimeout: timeout
                );

            return tables;
        }

        private async Task<List<ObjectPosition>> SelectBusObjectsPositionsAsync(IDbConnection connection, string sql, object param,
            IDbTransaction transaction, int? timeout)
        {
            var tables = new List<ObjectPosition>();

            var queryResult = await connection
                .QueryAsync<ObjectPosition, ObjectPosition, ObjectPosition>(
                    sql,
                    (t, s) =>
                    {
                        var table = tables.Where(i => i.Id == t.Id).FirstOrDefault();
                        if (table == null)
                        {
                            table = t;
                            table.Childs = new List<ObjectPosition>();
                            tables.Add(table);
                        }

                        table.Childs.Add(s);

                        return t;
                    },
                    param,
                    transaction,
                    commandTimeout: timeout
                );

            return tables;
        }


    }
}
