using BusTour.Data.Repositories.Tours.Queries;
using BusTour.Data.Repositories.Orders;
using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using BusTour.Domain.Models.Filters;
using Dapper;
using Infrastructure.Common.DI;
using Infrastructure.Db.Common;
using Infrastructure.Db.Common.Crud;
using Infrastructure.Db.Repositories;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using BusTour.Data.Repositories.NumberSequences;
using Infrastructure.Common.Configs;
using BusTour.Common.Config;

namespace BusTour.Data.Repositories.Tours
{
    [InjectAsSingleton]
    public class TourRepository : CrudRepository<Tour, TourQuery>, ITourRepository
    {
        private readonly ILogger _logger = LogManager.GetLogger(typeof(TourRepository).Name);
        private readonly IOrderRepository _orderRepository;
        private readonly INumberSequenceRepository _numberSequenceRepository;

        public TourRepository()
        {
            _logger = LogManager.GetCurrentClassLogger();
            _orderRepository = IoC.GetRequiredService<IOrderRepository>();
            _numberSequenceRepository = IoC.GetRequiredService<INumberSequenceRepository>();
        }

        public async Task<List<Tour>> SelectAsync(TourFilter filter)
        {
            var tours = await _db.QueryAsync<Tour>(FilterQueryObject.For(filter, SelectTourQuery.SelectByFilter, true));

            await FillNestedAsync(tours.ToArray());

            return tours.ToList();
        }

        public async Task<List<Tour>> SelectByIdAsync(TourFilter filter)
        {
            var tours = await _db.QueryAsync<Tour>(FilterQueryObject.For(filter, SelectTourQuery.SelectTourByFilter, true));

            await FillNestedAsync(tours.ToArray());

            return tours.ToList();
        }

        public async Task<TourGridModel> SelectTourFB(TourFilter filter)
        {
            var tours = new TourGridModel();
            tours.ToursInfo = (await _db.QueryAsync<TourInfoModel>(FilterQueryObject.For(filter, SelectTourQuery.SelectTourFB, true))).OrderBy(x=>x.Departure).ToList();
                     
            await FillNestedAsync(tours, filter);

            return tours;
        }

        public async Task<List<CurrentBookingGridModel>> SelectCurrentBookings(TourFilter filter)
        {
            var tours = await _db.QueryAsync<CurrentBookingGridModel>(FilterQueryObject.For(filter, SelectTourQuery.SelectTourFB, true));

            await FillNestedTourAsync(tours.ToArray(), filter);

            return tours.OrderBy(x => x.Departure).ToList();
        }

        protected override async Task FillNestedAsync(Tour[] tours)
        {
            foreach(var tour in tours)
            {
                tour.TourMenus = (await _db.QueryAsync<TourMenu>(new CrudQueryObject<TourMenu, TourMenuQuery>(new TourMenu { TourId = tour.Id }, CrudOperation.Select, true))).ToList();
                tour.TourBeverages = (await _db.QueryAsync<TourBeverage>(new CrudQueryObject<TourBeverage, TourBeverageQuery>(new TourBeverage { TourId = tour.Id }, CrudOperation.Select, true))).ToList();
                if (tour.RouteId.HasValue)
                {
                    tour.Route = await _db.QueryFirstOrDefaultAsync<Route>(FilterQueryObject.For(SelectTourQuery.SelectRoute, new Route { Id = tour.RouteId.Value }));
                }
                tour.PrivateHire = await _db.QueryFirstOrDefaultAsync<TourPrivateHire>(FilterQueryObject.For(SelectTourQuery.SelectPrivateHire, new TourPrivateHire { TourId = tour.Id }));
                tour.ServiceMaintenance = await _db.QueryFirstOrDefaultAsync<TourServiceMaintenance>(FilterQueryObject.For(SelectTourQuery.SelectServiceMaintenance, new TourServiceMaintenance { TourId = tour.Id }));
                tour.Tables = (await _db.ExecuteFuncAsync(GetTourInnerAsync, FilterQueryObject.For(new SelectTourQuery { Id = tour.Id }, SelectTourQuery.SelectTourNested))).Tables;
            }
        }

        protected async Task FillNestedAsync(TourGridModel tours, TourFilter filter)
        {
            foreach (var tour in tours.ToursInfo.ToArray())
            {
                var ids = new TourFilter { Ids = new List<int>() { tour.Id } };
                tour.GuestsNumber = await _db.QueryFirstOrDefaultAsync<int>(FilterQueryObject.For(ids, SelectTourQuery.SelectTourGuests, true));
            }
            tours.Menus = (await _db.QueryAsync<OrderMenu>(FilterQueryObject.For(filter, SelectTourQuery.SelectTourOrderMenu, true))).ToList();
            tours.ExtraMenus = (await _db.QueryAsync<OrderMenu>(FilterQueryObject.For(filter, SelectTourQuery.SelectTourOrderExtraMenu, true))).ToList();
            tours.Beverages = (await _db.QueryAsync<OrderBeverage>(FilterQueryObject.For(filter, SelectTourQuery.SelectTourOrderBeverage, true))).ToList();
            tours.ExtraBeverages = (await _db.QueryAsync<OrderBeverage>(FilterQueryObject.For(filter, SelectTourQuery.SelectTourOrderExtraBeverage, true))).ToList();
            tours.PrivateTourInfo = (await _db.QueryAsync<PrivateTourInfo>(FilterQueryObject.For(filter, SelectTourQuery.SelectPrivateTourInfo, true))).ToList();
            
        }

        protected async Task FillNestedTourAsync(CurrentBookingGridModel[] tours, TourFilter filter)
        {
            foreach (var tour in tours)
            {
                var tourOrders = await _orderRepository.SelectAsync(new OrderFilter { TourIds = new[] { tour.Id } });
                tour.Extras = tourOrders.Any(o => o.Beverages != null || o.Menus != null);
                //tour.GuestsNumber = tour.GuestsNumber ?? await _db.QueryFirstOrDefaultAsync<int>(FilterQueryObject.For(new TourFilter { Ids = new List<int>() { tour.Id } }, SelectTourQuery.SelectTourGuests, true));
                tour.SeatsNumber = await _db.QueryFirstOrDefaultAsync<int>(FilterQueryObject.For(new TourFilter { Ids = new List<int>() { tour.Id } }, SelectTourQuery.SelectTourSeats, true));
                tour.Paid = tourOrders.Where(x => x.OrderState == OrderState.Paid).Count();
                tour.Waiting = tourOrders.Where(x => x.OrderState == OrderState.WaitingForPayment).Count();
                tour.GuestsNumber = tour.GuestsNumber ?? tourOrders.Where(x => x.IsActive).Sum(x => x.GuestCount);
                int groupOrder = await _db.QueryFirstOrDefaultAsync<int>(FilterQueryObject.For(new TourFilter { Ids = new List<int>() { tour.Id } }, SelectTourQuery.SelectTourGroupOrder, true));
                tour.HasGroupOrder = groupOrder > 0 ? true : false;
            }

        }

        public override async Task<int> SaveOrUpdateAsync(Tour tour)
        {
            try
            {
                tour.Number = await _numberSequenceRepository.Increment(typeof(Tour), tour.Departure);

                var tourId = await base.SaveOrUpdateAsync(tour);

                tour.Id = tourId;

                //menus
                await _db.ExecuteAsync(new SqlQueryObject(TourQuery.DeleteTourMenus, new { TourId = tourId, MenuIds = tour.TourMenus.Select(x => x.MenuId) }));
                foreach (var tourMenu in tour.TourMenus)
                {
                    tourMenu.TourId = tourId;
                    await _db.ExecuteAsync(new CrudQueryObject<TourMenu, TourMenuQuery>(tourMenu, CrudOperation.Insert));
                }

                //beverages
                await _db.ExecuteAsync(new SqlQueryObject(TourQuery.DeleteTourBeverages, new { TourId = tourId, BeverageIds = tour.TourBeverages.Select(x => x.BeverageId) }));
                foreach (var tourBeverage in tour.TourBeverages)
                {
                    tourBeverage.TourId = tourId;
                    await _db.ExecuteAsync(new CrudQueryObject<TourBeverage, TourBeverageQuery>(tourBeverage, CrudOperation.Insert));
                }

                //private hire
                if (tour.PrivateHire != null)
                {
                    tour.PrivateHire.TourId = tourId;
                    await _db.ExecuteAsync(new SqlQueryObject(tour.PrivateHire.Id is default(int) ? TourQuery.InsertPrivateHire : TourQuery.UpdatePrivateHire, tour.PrivateHire));
                }

                //service maintenance
                if (tour.ServiceMaintenance != null)
                {
                    tour.ServiceMaintenance.TourId = tourId;
                    await _db.ExecuteAsync(new SqlQueryObject(tour.ServiceMaintenance.Id is default(int) ? TourQuery.InsertServiceMaintenance : TourQuery.UpdateServiceMaintenance, tour.ServiceMaintenance));
                }

                return tourId;
            }
            catch (Exception e)
            {
                _logger.Error(e, "CrudRepository.AddTourAsync threw error");
                throw;
            }
        }

        public async Task<Route> GetRouteAsync(short id)
        {
            try
            {
                //TODO: фильтр по ИД
                return await _db.ExecuteFuncAsync(GetRouteInnerAsync, FilterQueryObject.For(new SelectTourQuery(), SelectTourQuery.SelectRouteInfo));
                
            }
            catch (Exception e)
            {
                _logger.Error(e, "TourRepository.GetRoute threw error");
                throw;
            }
        }

        public async Task<List<Route>> GetRoutesAsync()
        {
            return (await _db.QueryAsync<Route>(new CrudQueryObject<Route, RouteQuery>(new Route(), CrudOperation.Select, true))).ToList();
        }

        public async Task<List<Bus>> GetBusesAsync()
        {
            return (await _db.QueryAsync<Bus>(new CrudQueryObject<int, Bus, BusQuery>(new Bus(), CrudOperation.Select, true))).ToList();
        }

        public async Task<List<City>> GetCitiesAsync()
        {
           return (await _db.QueryAsync<City>(FilterQueryObject.For<City>(SelectTourQuery.SelectCities))).ToList();
        }
        public async Task<Tour> GetTourAsync(int id)
        {
            try
            {
                return await _db.ExecuteFuncAsync(GetTourInnerAsync, FilterQueryObject.For(new SelectTourQuery { Id = id }, SelectTourQuery.SelectTourNested));
            }
            catch (Exception e)
            {
                _logger.Error(e, "TourRepository.GetTours threw error");
                throw;
            }
        }

        private async Task<Tour> GetTourInnerAsync(IDbConnection connection, string sql, object param,
           IDbTransaction transaction, int? timeout)
        {
            Tour tour = null;

            await connection.QueryAsync<Tour, Table, TableCategory, Seat, TourPrivateHire, TourServiceMaintenance, Tour>(
                sql,
                (t, tbl, tc, s, tph, tsm) =>
                {
                    if(tour == null)
                    {
                        tour = t;
                        tour.PrivateHire = tph;
                        tour.ServiceMaintenance = tsm;
                    }

                    if(tour.Tables == null)
                    {
                        tour.Tables = new List<Table>();
                    }

                    var table = tour.Tables.Where(i => i.Id == tbl.Id).FirstOrDefault();
                    if (table == null)
                    {
                        table = tbl;
                        table.Category = tc;
                        table.Seats = new List<Seat>();
                        tour.Tables.Add(table);
                    }

                    table.Seats.Add(s);

                    return t;
                },
                param,
                transaction,
                commandTimeout: timeout
            );

            return tour;
        }

        public async Task<List<Tour>> GetToursAsync(short routeId)
        {
            try
            {
                return await _db.ExecuteFuncAsync(GetToursInnerAsync, FilterQueryObject.For(new SelectTourQuery(), SelectTourQuery.SelectTours));
            }
            catch (Exception e)
            {
                _logger.Error(e, "TourRepository.GetTours threw error");
                throw;
            }
        }

        public async Task<List<Menu>> GetMenusAsync()
        {
            try
            {
                return await _db.ExecuteFuncAsync(GetMenusInnerAsync, FilterQueryObject.For(new SelectTourQuery(), SelectTourQuery.SelectMenus));
            }
            catch (Exception e)
            {
                _logger.Error(e, "TourRepository.GetMenus threw error");
                throw;
            }
        }

        public async Task<List<Beverage>> GetBeveragesAsync()
        {
            try
            {
                return await _db.ExecuteFuncAsync(GetBeveragesInnerAsync, FilterQueryObject.For(new SelectTourQuery(), SelectTourQuery.SelectBeverages));
            }
            catch (Exception e)
            {
                _logger.Error(e, "TourRepository.GetBeverages threw error");
                throw;
            }
        }

        public async Task<List<Allergy>> GetAllergiesAsync()
        {
            try
            {
                return await _db.ExecuteFuncAsync(GetAllergiesInnerAsync, FilterQueryObject.For(new SelectTourQuery(), SelectTourQuery.SelectAllergies));
            }
            catch (Exception e)
            {
                _logger.Error(e, "TourRepository.GetAllergies threw error");
                throw;
            }
        }

        public async Task<List<Surprise>> GetSurprisesAsync()
        {
            try
            {
                var result = await _db.QueryAsync<Surprise>(FilterQueryObject.For(new SelectTourQuery(), SelectTourQuery.SelectSurprises));
                return result.ToList();
            }
            catch (Exception e)
            {
                _logger.Error(e, "TourRepository.GetSurprises threw error");
                throw;
            }
        }

        public override async Task<Tour[]> GetAsync(params int[] ids)
        {
            Tour[] entities = null;
            await _db.ExecuteAsync(async (commands, ct) =>
            {
                var queryObject = GetDefaultQueryObject(ids);
                entities = (await commands.QueryAsync<Tour>(queryObject))
                    .ToArray();
            });
            await FillNestedAsync(entities);
            return entities ?? Array.Empty<Tour>();
        }

        public async Task UpdateServiceTourAsync(Tour tour)
        {
            await base.SaveOrUpdateAsync(tour);
            await _db.ExecuteAsync(new SqlQueryObject(TourQuery.UpdateServiceMaintenance, tour.ServiceMaintenance));
        }

        private async Task<Route> GetRouteInnerAsync(IDbConnection connection, string sql, object param,
            IDbTransaction transaction, int? timeout)
        {
            Route route = null;
            await connection.QueryAsync<Route, string, Route>(
                sql,
                (r, i) =>
                {
                    if (route == null)
                    {
                        route = r;
                        route.ImgPaths = new List<string>();
                    }

                    route.ImgPaths.Add(i);

                    return r;
                },
                param,
                transaction,
                commandTimeout: timeout,
                splitOn: "routeimgpath"
            );

            return route;
        }

        private async Task<List<Tour>> GetToursInnerAsync(IDbConnection connection, string sql, object param,
            IDbTransaction transaction, int? timeout)
        {
            var tours = new List<Tour>();

            await connection.QueryAsync<Tour, Table, TableCategory, Seat, Tour>(
                sql,
                (t, tbl, tc, s) =>
                {
                    var tour = tours.Where(i => i.Id == t.Id).FirstOrDefault();
                    if (tour == null)
                    {
                        tour = t;
                        tour.Tables = new List<Table>();
                        tours.Add(t);
                    }

                    var table = tour.Tables.Where(i => i.Id == tbl.Id).FirstOrDefault();
                    if (table == null)
                    {
                        table = tbl;
                        table.Category = tc;
                        table.Seats = new List<Seat>();
                        tour.Tables.Add(table);
                    }

                    table.Seats.Add(s);

                    return t;
                },
                param,
                transaction,
                commandTimeout: timeout
            );

            var nowDate = DateTime.UtcNow;
            var isNextDateBlocked = nowDate.TimeOfDay >= Config.Get<ApiConfig>().DeadlineTime;
            var nextAvailableDate = isNextDateBlocked
                ? new DateTime(nowDate.Year, nowDate.Month, nowDate.Day).AddDays(2)
                : new DateTime(nowDate.Year, nowDate.Month, nowDate.Day).AddDays(1);
            foreach(var tour in tours)
            {
                tour.IsAvailableForBooking = tour.TourState == TourState.Active && tour.Type == TourType.Regular
                    && tour.Departure >= nextAvailableDate && tour.Tables.Sum(x => x.Seats.Count) > tour.OccupiedSeatsCount;
            }

            return tours;
        }

        private async Task<List<Menu>> GetMenusInnerAsync(IDbConnection connection, string sql, object param,
            IDbTransaction transaction, int? timeout)
        {
            var menus = await connection.QueryAsync<Menu, MenuType, Menu>(
                sql,
                (m, t) =>
                {
                    m.MenuType = t;

                    return m;
                },
                param,
                transaction,
                commandTimeout: timeout
            );

            return menus.ToList();
        }

        private async Task<List<Beverage>> GetBeveragesInnerAsync(IDbConnection connection, string sql, object param,
            IDbTransaction transaction, int? timeout)
        {
            var beverages = await connection.QueryAsync<Beverage, BeverageGroup, WineType, Beverage>(
                sql,
                (b, g, wt) =>
                {
                    b.Group = g;
                    b.WineType = wt;

                    return b;
                },
                param,
                transaction,
                commandTimeout: timeout
            );

            return beverages.ToList();
        }

        private async Task<List<Allergy>> GetAllergiesInnerAsync(IDbConnection connection, string sql, object param,
           IDbTransaction transaction, int? timeout)
        {
            var allergies = await connection.QueryAsync<Allergy>(
                sql,
                param,
                transaction,
                commandTimeout: timeout
            );

            return allergies.ToList();
        }
    }
}
