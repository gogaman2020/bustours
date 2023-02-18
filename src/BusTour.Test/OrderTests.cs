using BusTour.AppServices.BookingService.Commands;
using BusTour.AppServices.BookingService.Queries;
using BusTour.AppServices.Payments.Commands;
using BusTour.AppServices.TourOrderProcess;
using BusTour.AppServices.TourOrderProcess.Args;
using BusTour.AppServices.TourOrderProcess.Commands;
using BusTour.Data.Repositories.BusRepository;
using BusTour.Data.Repositories.Orders;
using BusTour.Data.Repositories.Tours;
using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using BusTour.Domain.Models.Filters;
using BusTour.Domain.Models.Order;
using BusTour.Domain.Models.Responses;
using Infrastructure.Common.DI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusTour.Test
{
    [TestClass]
    public class OrderTests : UnitTestBase
    {
        [TestMethod]
        public async Task OrderPaymentSuccess()
        {
            var orderRepository = IoC.GetRequiredService<IOrderRepository>();

            var waitingOrder = (await orderRepository.SelectAsync(new OrderFilter { States = new List<OrderState> { OrderState.WaitingForPayment } })).FirstOrDefault();

            await _mediator.RunCommandAsync(new OrderPaymentSuccessCommand(waitingOrder.Id, null), async result =>
            {
                var payment = result.Result;

                Assert.IsTrue(payment.Id != default(int));

                var order = await orderRepository.GetAsync(payment.OrderId.Value);

                Assert.IsTrue(order.OrderState == OrderState.Paid);
            });
        }

        [TestMethod]
        public async Task OrderPaymentFail()
        {
            var orderRepository = IoC.GetRequiredService<IOrderRepository>();

            var waitingOrder = (await orderRepository.SelectAsync(new OrderFilter { States = new List<OrderState> { OrderState.WaitingForPayment } })).FirstOrDefault();

            var error = "errrr";

            await _mediator.RunCommandAsync(new OrderPaymentFailCommand(waitingOrder.Id, error), async result =>
            {
                var payment = result.Result;

                Assert.IsTrue(payment.Id != default(int));

                var order = await orderRepository.GetAsync(payment.OrderId.Value);

                Assert.IsTrue(order.OrderState == OrderState.NotPaid);
                Assert.IsTrue(order.Payment.Details.Error == error);
            });
        }

        [TestMethod]
        public async Task CheckOrderseConflictsByFilter()
        {
            var conflicts = (await _mediator.RunCommandAsync(new CheckOrdersConflictsQuery(new OrderFilter
            {
                TourIds = new List<int> { 177 }
            }))).Result;

            Assert.IsTrue(conflicts.Any(x => x.ConflictSeatIds.Any()), "Has conflicts");
        }

        [TestMethod]
        public async Task CheckPrivateHireConflicts()
        {
            var tourRepository = IoC.GetRequiredService<ITourRepository>();
            var orderRepository = IoC.GetRequiredService<IOrderRepository>();

            var paidOrders = await orderRepository.SelectAsync(new Domain.Models.Filters.OrderFilter
            {
                States = new List<OrderState> {  OrderState.Paid }
            });

            if (paidOrders.Any())
            {
                var conflicts = (await _mediator.RunCommandAsync(new CheckOrderConflictsQuery(new OrderModel
                {
                    Type = OrderType.PrivateHire,
                    PrivateHire = new OrderPrivateHireModel
                    {
                        Date = paidOrders.First().Tour.Departure,
                        IsAllDay = true
                    }
                }))).Result;

                Assert.IsTrue(conflicts.Any(), "Has conflicts");
            }
            else
            {
                Assert.IsTrue(false, "No paid orders");
            }
        }

        [TestMethod]
        public async Task AddPrivateHireOrder()
        {
            var tourRepository = IoC.GetRequiredService<ITourRepository>();
            var orderRepository = IoC.GetRequiredService<IOrderRepository>();
            var busRepository = IoC.GetRequiredService<IBusRepository>();

            var menus = await tourRepository.GetMenusAsync();
            var beverages = await tourRepository.GetBeveragesAsync();

            var buses = await busRepository.GetBusesAsync();

            var orderModel = new OrderModel
            {
                Type = OrderType.PrivateHire,
                Client = new OrderClientModel
                {
                    FullName = "James Bond",
                    PhoneNumber = "123456789",
                    Email = "aaa"
                },
                Menus = menus.Take(3).Select(x => new OrderMenuModel
                {
                    MenuId = x.Id,
                    Amount = 2
                }).ToList(),
                Beverages = beverages.Take(2).Select(x => new OrderBeverageModel
                {
                    BeverageId = x.Id,
                    Amount = 3
                }).ToList(),
                PrivateHire = new OrderPrivateHireModel
                {
                    Date = DateTime.Today.AddDays(1),
                    IsAllDay = true,
                    DeparturePoint = "Abbey Road",
                    ArrivalPoint = "Baker Street",
                    BusId = buses.First().Id,
                    TourPrice = 777,
                    Stops = new List<string> { "First", "Second", "Third" }

                }
            };

            var order = (await _mediator.RunCommandAsync(new CreateOrUpdatePrivateHireOrderCommand(orderModel), async result => 
            {
                var order = result.Result;

                Assert.IsTrue(order?.Id != default(int), "Order created");

                //Assert.IsTrue(order?.Menus.Sum(x => x.Amount) == orderModel.Menus.Sum(x => x.Amount), "Order menus");
                //Assert.IsTrue(order?.Beverages.Sum(x => x.Amount) == orderModel.Beverages.Sum(x => x.Amount), "Order beverages");

                Assert.IsTrue(order?.Tour?.Id != default(int), "Tour created");

                var tour = await tourRepository.GetAsync(order.Tour.Id);

                Assert.IsTrue(tour?.PrivateHire?.Id != default(int), "Private hire created");

                Assert.IsTrue(tour?.PrivateHire?.Stops.Where(x => x != "").Count() == orderModel.PrivateHire.Stops.Count, "Private hire stops");

            })).Result;
        }

        [TestMethod]
        public async Task GetCalculationCostTour()
        {
            var model = new OrderModel
            {
                PromoCodeId = 102,
                CertificateId = 692,
                TourId = 7096,
                Seats = new List<OrderSeatModel>
                {
                    new OrderSeatModel
                    {
                        SeatId = 13,
                        MenuId = 1,
                        BeverageId = 1,
                    },
                    new OrderSeatModel
                    {
                        SeatId = 14,
                        MenuId = 1,
                        BeverageId = 1,
                    },
                    new OrderSeatModel
                    {
                        SeatId = 15,
                        MenuId = 1,
                        BeverageId = 1,
                    },
                    new OrderSeatModel
                    {
                        SeatId = 16,
                    },
                },
                Tables = new List<OrderTable>
                {
                    new OrderTable { TableId = 6 }
                },
                Menus = new List<OrderMenuModel>
                {
                    new OrderMenuModel { MenuId = 5, Amount = 5 }
                },
                Beverages = new List<OrderBeverageModel>
                {
                    new OrderBeverageModel { BeverageId = 7 , Amount = 5 },
                    new OrderBeverageModel { BeverageId = 8 , Amount = 5 },
                    new OrderBeverageModel { BeverageId = 11 , Amount = 5 },
                }
            };

            var result = (await _mediator.RunCommandAsync(new GetCalculationCostTourQuery(model))).Result;

            Assert.IsTrue(
                Math.Round(result.TotalPrice, 0) == 632 &&
                Math.Round(result.TourPrice, 0) == 200 &&
                Math.Round(result.VAT, 0) == 130
                );
        }
    }
}
