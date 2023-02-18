using BusTour.AppServices.TourService.Commands;
using BusTour.AppServices.TourService.Queries;
using BusTour.Data.Repositories.BusRepository;
using BusTour.Data.Repositories.Orders;
using BusTour.Data.Repositories.Tours;
using BusTour.Domain.Enums;
using BusTour.Domain.Models;
using BusTour.Domain.Models.Filters;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BusTour.Test
{
    [TestClass]
    public class ToursTest : UnitTestBase
    {
        [TestMethod]
        public async Task CreateTours()
        {
            var mediator = IoC.GetRequiredService<IMediator>();
            var tourRepository = IoC.GetRequiredService<ITourRepository>();

            var menus = await tourRepository.GetMenusAsync();
            var beverages = await tourRepository.GetBeveragesAsync();

            var command = new CreateToursCommand
            {
                DateStart = new DateTime(2021, 11, 22),
                DateEnd = new DateTime(2021, 11, 28),
                Type = TourType.Regular,

                Tours = new List<CreateToursCommand.CreateToursTour>
                {
                    new CreateToursCommand.CreateToursTour
                    {
                        WeekdayStart = DayOfWeek.Monday,
                        WeekdayEnd = DayOfWeek.Friday,
                        SeatPrice = 111,
                        VipPrice = 222,
                        Discount = 0.3m,
                        Menus = menus.Take(3).Select(x => x.Id).ToList(),
                        Beverages =  beverages.Take(4).Select(x => x.Id).ToList(),
                        HasMenu = true,
                        HasBeverages = true,
                        Times = new List<Time?>
                        {
                            new Time { Hours = 8 },
                            new Time { Hours = 14 }
                        }
                    }
                }
            };

            await mediator.RunCommandAsync(command, async result =>
            {
                var tours = result.Result;
                Assert.AreEqual(10, tours.Count);
                Assert.IsTrue(tours.First().TourMenus.Count() == command.Tours.First().Menus.Count());
                Assert.IsTrue(tours.First().TourBeverages.Count() == command.Tours.First().Beverages.Count());
            });

            command.Type = TourType.Service;

            await mediator.RunCommandAsync(command, async result =>
            {
                var tours = result.Result;
                Assert.AreEqual(10, tours.Count);
                Assert.AreEqual(0, tours.First().TourMenus.Count);
                Assert.AreEqual(0, tours.First().TourBeverages.Count);
            });
        }

        [TestMethod]
        public async Task CreateServiceTour()
        {
            var orderRepository = IoC.GetRequiredService<IOrderRepository>();
            var tourRepository  = IoC.GetRequiredService<ITourRepository>();

            var dateStart = DateTime.UtcNow.Date.AddDays(1);

            var command = new CreateToursCommand
            {
                Type = TourType.Service,

                Tours = new List<CreateToursCommand.CreateToursTour>
                {
                    new CreateToursCommand.CreateToursTour
                    {
                        ServiceStart = new Time(10),
                        ServiceEnd   = new Time(14),
                    }
                }
            };

            await _mediator.RunCommandAsync(command, async result =>
            {
                var tours = result.Result;

                var orders = await orderRepository.SelectAsync(new OrderFilter { TourIds = tours.Select(x => x.Id) });

                Assert.IsTrue(orders.All(x => x.Type == OrderType.Service));
                Assert.AreEqual(orders.Count, tours.Count);
            });
        }

        [TestMethod]
        public async Task FilterTours()
        {
            var mediator = IoC.GetRequiredService<IMediator>();

            var filter = new TourFilter
            {
                TourTypes = new List<TourType> { TourType.Regular, TourType.Service }
            };

            var all = (await mediator.RunCommandAsync(new FilterToursQuery(filter), async result =>
            {
                Assert.IsTrue(result.Result.Any(z => z.RouteId != default(int)), "RouteId");
                Assert.IsTrue(filter.TourTypes.Count() == result.Result.GroupBy(z => z.Type).Count(), "TourTypes");
            })).Result;

            filter = new TourFilter
            {
                DepartureDateFrom = all.Min(x => x.Departure).AddDays(10),
                DepartureDateTo = all.Max(x => x.Departure).AddDays(-10),
            };

            await mediator.RunCommandAsync(new FilterToursQuery(filter), async result =>
            {
                Assert.IsTrue(filter.DepartureDateFrom <= result.Result.Min(x => x.Departure), "DateFrom");
                Assert.IsTrue(filter.DepartureDateTo >= result.Result.Min(x => x.Departure), "DateTo");
            });

            filter = new TourFilter
            {
                Limit = 10,
                Offset = 0
            };

            var id = 0;

            await mediator.RunCommandAsync(new FilterToursQuery(filter), async result =>
            {
                id = result.Result.First().Id;
                Assert.IsTrue(filter.Limit >= result.Result.Count(), "Limit");
            });

            filter = new TourFilter
            {
                Ids = new List<int> { id }
            };

            await mediator.RunCommandAsync(new FilterToursQuery(filter), async result =>
            {
                Assert.IsTrue(filter.Ids.Count() == result.Result.Count(), "Ids");
            });
        }

        [TestMethod]
        public async Task Delete()
        {
            var tours = (await _mediator.RunCommandAsync(new FilterToursQuery(new TourFilter { Limit = 2, HasOrders = false }))).Result;

            Debug.WriteLine($"delete {string.Join(",", tours.Select(x => x.Id))}", OutputLevel.Information);

            await _mediator.RunCommandAsync(new DeleteToursCommand(tours.Select(x => x.Id).ToList()), async result =>
            {
                Assert.IsTrue(result.Result.Count() == tours.Count && result.Result.All(z => tours.Select(x => x.Id).Contains(z)), "Delete");
            });
        }

        [TestMethod]
        public async Task SelectBuses()
        {
            var busRepository = IoC.GetRequiredService<IBusRepository>();

            var buses = await busRepository.GetBusesAsync();

            Assert.IsTrue(buses.Any());
            Assert.IsTrue(buses.First().Tables.Any());
            Assert.IsTrue(buses.First().Tables.First().Seats.Any());
        }

        [TestMethod]
        public async Task SelectBus()
        {
            var busRepository = IoC.GetRequiredService<IBusRepository>();

            var buses = await busRepository.GetBusesAsync();

            var bus = await busRepository.GetAsync(buses.First().Id);

            Assert.IsTrue(bus != null);
            Assert.IsTrue(bus.Tables.Any());
            Assert.IsTrue(bus.Tables.First().Seats.Any());
        }
    }
}
