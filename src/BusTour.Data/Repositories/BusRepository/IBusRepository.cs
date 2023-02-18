using BusTour.Domain.Entities;
using BusTour.Domain.Models.Filters;
using Infrastructure.Db.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusTour.Data.Repositories.BusRepository
{
    public interface IBusRepository: ICrudRepository<Bus>
    {
        /// <summary>
        /// Получить сиденья автобуса
        /// </summary>
        /// <param name="busIds"></param>
        /// <returns></returns>
        Task<List<Seat>> SelectSeatAsync(SeatFilter filter);

        /// <summary>
        /// Получить автобусы
        /// </summary>
        /// <returns></returns>
        Task<List<Bus>> GetBusesAsync();
    }
}
