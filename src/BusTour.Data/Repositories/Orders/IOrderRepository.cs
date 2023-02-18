using BusTour.Domain.Entities;
using BusTour.Domain.Models.Filters;
using BusTour.Domain.Models.Order;
using Infrastructure.Db.Repositories;
using Infrastructure.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusTour.Data.Repositories.Orders
{
    public interface IOrderRepository
    {
        Task<int> UpdateAsync(Order order);

        Task UpdateOrderSeat(OrderSeat orderSeat);

        Task UpdateOrderMenu(OrderMenu orderMenu);

        Task UpdateOrderBeverage(OrderBeverage orderBeverage);

        Task AllGuestHasComeAsync(int orderId);

        Task<int> SaveOrUpdateAsync(Order order);

        Task DeleteNestedAsync(Order order);

        Task<List<int>> TryInsertOrderAsync(Order order);

        Task<List<int>> GetSeatsByTablesAsync(List<int> tableIds);

        Task<Order> GetAsync(int id);

        Task<Order> GetAsync(int id, bool fillNested);

        Task<List<Order>> SelectAsync(OrderFilter filter);

        async Task<Order> FindAsync(OrderFilter filter)
        {
            return (await SelectAsync(filter)).FirstOrDefault();
        }

        Task<OrderExtras> GetExtrasAsync(int id);

        /// <summary>
        /// Удаляем сиденье заказа.
        /// </summary>
        /// <returns></returns>
        Task DeleteOrderSeatAsync(int orderSeatId);

        /// <summary>
        /// Добавляем сиденье заказа.
        /// </summary>
        /// <returns></returns>
        Task AddOrderSeatAsync(OrderSeat orderSeat);
    }
}
