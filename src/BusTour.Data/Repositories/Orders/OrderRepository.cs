using BusTour.Data.Repositories.GiftCertificates;
using BusTour.Data.Repositories.Orders.Queries;
using BusTour.Data.Repositories.PromoCodes;
using BusTour.Data.Repositories.Tours;
using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using BusTour.Domain.Models.Filters;
using BusTour.Domain.Models.Order;
using Dapper;
using Infrastructure.Common.DI;
using Infrastructure.Db.Common;
using Infrastructure.Db.Common.Crud;
using Infrastructure.Db.Common.Transactions;
using Infrastructure.Db.Repositories;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BusTour.Data.Repositories.Orders
{
    [InjectAsSingleton]
    public class OrderRepository : CrudRepository<Order, OrderQuery>, IOrderRepository
    {
        private readonly ILogger _logger;
        private readonly ITransactionFactory _txFactory;

        public OrderRepository(ITransactionFactory txFactory)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _txFactory = txFactory;
        }

        public override async Task<int> SaveOrUpdateAsync(Order order)
        {
            await SaveClient(order);

            order.Id = await base.SaveOrUpdateAsync(order);

            await SaveNestedAsync(order);

            return order.Id;
        }

        public async Task<int> UpdateAsync(Order order)
        {
            await SaveClient(order);

            order.Id = await base.SaveOrUpdateAsync(order);

            return order.Id;
        }

        public async Task UpdateOrderSeat(OrderSeat seat)
        {
            await _db.ExecuteAsync(new CrudQueryObject<OrderSeat, OrderSeatQuery>(seat, CrudOperation.Update));
        }
        public async Task UpdateOrderBeverage(OrderBeverage orderBeverage)
        {
            await _db.ExecuteAsync(new CrudQueryObject<OrderBeverage, OrderBeverageQuery>(orderBeverage, CrudOperation.Update));
        }
        public async Task UpdateOrderMenu(OrderMenu orderMenu)
        {
            await _db.ExecuteAsync(new CrudQueryObject<OrderMenu, OrderMenuQuery>(orderMenu, CrudOperation.Update));
        }

        public async Task AllGuestHasComeAsync(int orderId)
        {
            var query = new OrderQuery { OrderId = orderId };
            await _db.QueryAsync<int>(FilterQueryObject.For(query, OrderSeatQuery.AllGuestHasCome));
        }

        public async Task DeleteNestedAsync(Order order)
        {
            await _db.ExecuteAsync(new CrudQueryObject<OrderSeat, OrderSeatQuery>(new OrderSeat { OrderId = order.Id }, CrudOperation.Delete));

            await _db.ExecuteAsync(new CrudQueryObject<OrderMenu, OrderMenuQuery>(new OrderMenu { OrderId = order.Id }, CrudOperation.Delete));
            
            await _db.ExecuteAsync(new CrudQueryObject<OrderBeverage, OrderBeverageQuery>(new OrderBeverage { OrderId = order.Id }, CrudOperation.Delete));
        }

        public async Task<List<int>> TryInsertOrderAsync(Order order)
        {
            try
            {
                return await _txFactory.UseTransactionWithRetryAsync(async t =>
                {
                    var query = new OrderQuery { TourId = order.TourId };
                    var occupiedSeatIds = await _db.QueryAsync<int>(FilterQueryObject.For(query, OrderQuery.SelectLockedSeats));

                    var lockedSeats = occupiedSeatIds.Where(p => order.Seats.Any(s => s.SeatId == p)).ToList();

                    if (lockedSeats.Any())
                    {
                        return lockedSeats;
                    }

                    await SaveClient(order);
                    
                    order.Id = await base.SaveOrUpdateAsync(order);

                    await SaveNestedAsync(order);

                    t.Commit();

                    return new List<int>();
                }, keyLock: nameof(TryInsertOrderAsync));
            }
            catch (Exception e)
            {
                _logger.Error(e, "OrderRepository.TryInsertOrder threw error");
                throw;
            }
        }

        public async Task<List<int>> GetSeatsByTablesAsync(List<int> tableIds)
        {
            try
            {
                return (await _db.QueryAsync<int>(FilterQueryObject.For(new OrderSeatQuery 
                { 
                    TableIds = tableIds
                }, OrderSeatQuery.GetSeatsByTables))).ToList();
            }
            catch (Exception e)
            {
                _logger.Error(e, "OrderRepository.GetSeatsByTables threw error");
                throw;
            }
        }

        public override async Task<Order[]> GetAsync(params int[] ids)
        {
            Order[] entities = null;
            await _db.ExecuteAsync(async (commands, ct) =>
            {
                var queryObject = GetDefaultQueryObject(ids);
                entities = (await commands.ExecuteFuncAsync(SelectInnerAsync, queryObject))
                .ToArray();
            });
            await FillNestedAsync(entities);
            return entities ?? Array.Empty<Order>();
        }

        public override async Task<Order> GetAsync(int id, bool fillNested)
        {
            var queryObject = GetDefaultQueryObject(new Order() { Id = id }, CrudOperation.Select);
            var order = await _db.QueryFirstOrDefaultAsync<Order>(queryObject);

            if (order == null)
            {
                return order;
            }
            order.Id = id;

            if (fillNested)
            {
                await this.FillNestedAsync(new Order[] { order });
            }

            return order;
        }

        public async Task<List<Order>> SelectAsync(OrderFilter filter)
        {
            var orders = new List<Order>();
            await _db.ExecuteAsync(async (commands, ct) =>
            {
                orders = (await commands.ExecuteFuncAsync(SelectInnerAsync, FilterQueryObject.For(filter, OrderQuery.SelectByFilter, true))).ToList();
            });
            await FillNestedAsync(orders.ToArray());
            return orders;
        }

        protected override async Task FillNestedAsync(Order[] orders)
        {
            var promoCodeRepo = IoC.GetRequiredService<IPromoCodeRepository>();
            var giftCertificateRepo = IoC.GetRequiredService<IGiftCertificateRepository>();

            foreach (var order in orders)
            {
                var orderSeatQuery = new CrudQueryObject<OrderSeat, OrderSeatQuery>(new OrderSeat { OrderId = order.Id }, CrudOperation.Select, true);
                order.Seats = (await _db.QueryAsync<OrderSeat>(orderSeatQuery)).ToList();

                var orderBeverageQuery = new CrudQueryObject<OrderBeverage, OrderBeverageQuery>(new OrderBeverage { OrderId = order.Id }, CrudOperation.Select, true);
                order.Beverages = (await _db.QueryAsync<OrderBeverage>(orderBeverageQuery)).ToList();

                var orderSurpriseQuery = new CrudQueryObject<OrderSurprise, OrderSurpriseQuery>(new OrderSurprise { OrderId = order.Id }, CrudOperation.Select, true);
                order.Surprises = (await _db.QueryAsync<OrderSurprise>(orderSurpriseQuery)).ToList();

                var orderMenuQuery = new CrudQueryObject<OrderMenu, OrderMenuQuery>(new OrderMenu { OrderId = order.Id }, CrudOperation.Select, true);
                order.Menus = (await _db.QueryAsync<OrderMenu>(orderMenuQuery)).ToList();

                if (order.PromoCodeId.HasValue)
                {
                    order.PromoCode = await promoCodeRepo.GetAsync(order.PromoCodeId.Value);
                }

                if (order.CertificateId.HasValue)
                {
                    order.GiftCertificate = await giftCertificateRepo.GetAsync(order.CertificateId.Value);
                }

                order.Tour = await IoC.GetRequiredService<ITourRepository>().GetAsync(order.TourId);
            }
        }

        public async Task SaveClient(Order order)
        {
            if (order.Client != null)
            {
                order.Client.PhoneNumber = new string(order.Client.PhoneNumber?.Where(c => char.IsDigit(c)).ToArray());
                order.Client.Id = await _db.QueryFirstOrDefaultAsync<int>(FilterQueryObject.For(order.Client, ClientQuery.UpsertCommand));
                order.ClientId = order.Client.Id;
            }
        }

        private async Task SaveNestedAsync(Order order)
        {
            var commands = new List<IQueryObject>();

            commands.Add(DeleteOrderSeats(order.Id));
            commands.AddRange(order.Seats.Select(p => InsertOrderSeat(order.Id, p)));

            commands.Add(DeleteOrderMenus(order.Id));
            commands.AddRange(order.Menus.Where(p => p.Amount > 0).Select(p => InsertOrderMenu(order.Id, p)));

            commands.Add(DeleteOrderBeverages(order.Id));
            commands.AddRange(order.Beverages.Where(p => p.Amount > 0).Select(p => InsertOrderBeverage(order.Id, p)));

            commands.AddRange(order.Surprises.Where(p => p.Amount > 0).Select(p => InsertOrderSurprise(order.Id, p)));

            await _db.ExecuteAsync(commands.ToArray());
        }

        private Task<IEnumerable<Order>> SelectInnerAsync(IDbConnection connection, string sql, object param, IDbTransaction transaction, int? timeout)
        {
            return connection
                .QueryAsync<Order, Client, Payment, Order>(
                    sql,
                    (order, client, payment) =>
                    {
                        order.Client = client;
                        order.Payment = payment;

                        //order.Tour   = tour;
                        //if (order.Tour != null)
                        //{
                        //    order.Tour.Route = route;
                        //    order.Tour.PrivateHire = privateHire;
                        //}

                        return order;
                    },
                    param,
                    transaction,
                    commandTimeout: timeout
                );
        }

        public async Task<OrderExtras> GetExtrasAsync(int id)
        {
            var extras = new OrderExtras();

            var orderBeverageQuery = new CrudQueryObject<OrderBeverage, OrderBeverageQuery>(new OrderBeverage { OrderId = id }, CrudOperation.Select, true);
            extras.Beverages = (await _db.QueryAsync<OrderBeverage>(orderBeverageQuery)).ToList();

            var orderMenuQuery = new CrudQueryObject<OrderMenu, OrderMenuQuery>(new OrderMenu { OrderId = id }, CrudOperation.Select, true);
            extras.Menus = (await _db.QueryAsync<OrderMenu>(orderMenuQuery)).ToList();

            return extras;
        }
        public Task DeleteOrderSeatAsync(int orderSeatId)
        {
            return _db.ExecuteAsync(new List<IQueryObject> { DeleteOrderSeat(orderSeatId) }.ToArray());
        }

        public Task AddOrderSeatAsync(OrderSeat orderSeat)
        {
            return _db.ExecuteAsync(new List<IQueryObject> { InsertOrderSeat(orderSeat.OrderId.Value, orderSeat) }.ToArray());
        }

        private IQueryObject DeleteOrderSeat(int orderSeatId)
        {
            return FilterQueryObject.For(new
            {
                Id = orderSeatId
            }, OrderSeatQuery.Delete);
        }

        private IQueryObject InsertOrderSeat(int orderId, OrderSeat seat)
        {
            var entity = new
            {
                OrderId = orderId,
                seat.SeatId,
                seat.MenuId,
                seat.BeverageId,
                seat.AllergyId,
                seat.OtherAllergy,
                seat.IsEmpty,
                seat.Price
            };
            return FilterQueryObject.For(entity, OrderSeatQuery.Insert);
        }
        private IQueryObject DeleteOrderSeats(int orderId)
        {
            return FilterQueryObject.For(new
            {
                OrderId = orderId
            }, OrderSeatQuery.Delete);
        }

        private IQueryObject DeleteOrderMenus(int orderId)
        {
            return FilterQueryObject.For(new
            {
                OrderId = orderId
            }, OrderMenuQuery.Delete);
        }

        private IQueryObject InsertOrderMenu(int orderId, OrderMenu menu)
        {
            var entity = new
            {
                OrderId = orderId,
                menu.MenuId,
                menu.Amount
            };
            return FilterQueryObject.For(entity, OrderMenuQuery.Insert);
        }

        private IQueryObject DeleteOrderBeverages(int orderId)
        {
            return FilterQueryObject.For(new
            {
                OrderId = orderId
            }, OrderBeverageQuery.Delete);
        }

        private IQueryObject InsertOrderBeverage(int orderId, OrderBeverage beverage)
        {
            var entity = new
            {
                OrderId = orderId,
                beverage.BeverageId,
                beverage.Amount
            };
            return FilterQueryObject.For(entity, OrderBeverageQuery.Insert);
        }

        private IQueryObject InsertOrderSurprise(int orderId, OrderSurprise surprise)
        {
            var entity = new
            {
                OrderId = orderId,
                surprise.SurpriseId,
                surprise.Amount
            };
            return FilterQueryObject.For(entity, OrderSurpriseQuery.Insert);
        }
    }
}
