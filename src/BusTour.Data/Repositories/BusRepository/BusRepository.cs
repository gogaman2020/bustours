using BusTour.Data.Repositories.BusRepository.Queries;
using BusTour.Domain.Entities;
using BusTour.Domain.Models.Filters;
using Dapper;
using Infrastructure.Common.DI;
using Infrastructure.Db.Common;
using Infrastructure.Db.Repositories;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusTour.Data.Repositories.BusRepository
{
    [InjectAsSingleton]
    public class BusRepository : CrudRepository<Bus, BusQuery>, IBusRepository
    {
        private readonly ILogger _logger = LogManager.GetLogger(typeof(BusRepository).Name);

        public override async Task<Bus> GetAsync(int id)
        {
            try
            {
                return (await _db.ExecuteFuncAsync(SelectBusesInnerAsync, FilterQueryObject.For(new { Ids = new List<int> { id } }, BusQuery.SelectBus))).FirstOrDefault();
            }
            catch (Exception e)
            {
                _logger.Error(e, "TourRepository.GetTours threw error");
                throw;
            }
        }

        public async Task<List<Bus>> GetBusesAsync()
        {
            try
            {
                return await _db.ExecuteFuncAsync(SelectBusesInnerAsync, FilterQueryObject.For<Bus>(BusQuery.SelectBus));
            }
            catch (Exception e)
            {
                _logger.Error(e, "TourRepository.GetTours threw error");
                throw;
            }
        }

        public async Task<List<Seat>> SelectSeatAsync(SeatFilter filter)
        {
            var seats = new List<Seat>();
            await _db.ExecuteAsync(async (commands, ct) =>
            {
                seats = (await commands.ExecuteFuncAsync(SelectSeatInnerAsync, FilterQueryObject.For(filter, BusQuery.SelectSeat))).ToList();
            });
            return seats;
        }

        private Task<IEnumerable<Seat>> SelectSeatInnerAsync(IDbConnection connection, string sql, object param, IDbTransaction transaction, int? timeout)
        {
            return connection
                .QueryAsync<Seat, Table, TableCategory, Seat> (
                    sql,
                    (seat, table, tableCategory) =>
                    {
                        table.Category = tableCategory;
                        seat.Table = table;
                        return seat;
                    },
                    param,
                    transaction,
                    commandTimeout: timeout
                );
        }

        private async Task<List<Bus>> SelectBusesInnerAsync(IDbConnection connection, string sql, object param, IDbTransaction transaction, int? timeout)
        {
            var buses = new List<Bus>();

            await connection.QueryAsync<Bus, Table, TableCategory, Seat, Bus>(
                sql,
                (b, tbl, tblc, s) =>
                {
                    var bus = buses.FirstOrDefault(x => x.Id == b.Id);
                    if (bus == null)
                    {
                        bus = b;
                        buses.Add(bus);
                    }

                    var table = bus.Tables.FirstOrDefault(x => x.Id == tbl.Id);
                    if (table == null)
                    {
                        table = tbl;
                        table.Category = tblc;
                        bus.Tables.Add(table);
                    }

                    var seat = table.Seats.FirstOrDefault(x => x.Id == s.Id);
                    if (seat == null)
                    {
                        seat = s;
                        table.Seats.Add(seat);
                    }

                    return bus;
                },
                param,
                transaction,
                commandTimeout: timeout
            );

            return buses;
        }

    }
}
