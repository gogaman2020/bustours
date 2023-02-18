using BusTour.AppServices.TourOrderProcess;
using BusTour.AppServices.TourOrderProcess.Args;
using BusTour.AppServices.TourOrderProcess.Commands;
using BusTour.AppServices.TourOrderProcess.Steps;
using BusTour.Data.Repositories.Orders;
using BusTour.Domain.Entities;
using BusTour.Domain.Models.Auth;
using Infrastructure.Common.DI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace BusTour.Test
{
    [TestClass]
    public class TourOrderProcessTest: UnitTestBase
    {
        [TestMethod]
        public async Task ProcessOrderCancelAsync()
        {
            SetContext(1, Role.Crew);
            var orderId = await InitOrderAsync();
            var process = IoC.GetRequiredService<ITourOrderProcess>();

            process.Reset();
            await process.SetContextAsync(orderId);
            Assert.IsTrue(process.CurrentStepName == nameof(TourOrderDraftStep));

            await process.SendCommandAsync(TourOrderStepCommand.Cancel);
            process.Reset();
            await process.SetContextAsync(orderId);
            Assert.IsTrue(process.CurrentStepName == nameof(TourOrderCanceledStep));
        }

        [TestMethod]
        public async Task ProcessOrderNotPaidAsync()
        {
            SetContext(1, Role.Crew);
            var orderId = await InitOrderAsync();
            var process = IoC.GetRequiredService<ITourOrderProcess>();

            process.Reset();
            await process.SetContextAsync(orderId);
            Assert.IsTrue(process.CurrentStepName == nameof(TourOrderDraftStep));

            await process.SendCommandAsync(TourOrderStepCommand.WaitingForPaiment);
            process.Reset();
            await process.SetContextAsync(orderId);
            Assert.IsTrue(process.CurrentStepName == nameof(TourOrderWaitingForPaymentStep));

            await process.SendCommandAsync(new PayStepCommandArgs(TourOrderStepCommand.Payment) { IsPaid = false });
            process.Reset();
            await process.SetContextAsync(orderId);
            Assert.IsTrue(process.CurrentStepName == nameof(TourOrderNotPaidStep));
        }

        [TestMethod]
        public async Task ProcessOrderPayNadCancelAsync()
        {
            SetContext(1, Role.Crew);
            var orderId = await InitOrderAsync();
            var process = IoC.GetRequiredService<ITourOrderProcess>();

            process.Reset();
            await process.SetContextAsync(orderId);
            Assert.IsTrue(process.CurrentStepName == nameof(TourOrderDraftStep));

            await process.SendCommandAsync(TourOrderStepCommand.WaitingForPaiment);
            process.Reset();
            await process.SetContextAsync(orderId);
            Assert.IsTrue(process.CurrentStepName == nameof(TourOrderWaitingForPaymentStep));

            await process.SendCommandAsync(new PayStepCommandArgs(TourOrderStepCommand.Payment) { IsPaid = true });
            process.Reset();
            await process.SetContextAsync(orderId);
            Assert.IsTrue(process.CurrentStepName == nameof(TourOrderPaidStep));

            await process.SendCommandAsync(TourOrderStepCommand.Cancel);
            process.Reset();
            await process.SetContextAsync(orderId);
            Assert.IsTrue(process.CurrentStepName == nameof(TourOrderCanceledStep));
        }

        private async Task<int> InitOrderAsync()
        {
            var order = new Order
            {
                Beverages = new System.Collections.Generic.List<OrderBeverage>
                {
                    new OrderBeverage
                    {
                        Amount = 1,
                        BeverageId = 1
                    }
                },
                Client = new Client
                {
                    Email = string.Empty,
                    FullName = string.Empty,
                    PhoneNumber = string.Empty,
                },
                Menus = new System.Collections.Generic.List<OrderMenu>
                {
                    new OrderMenu
                    {
                        Id = 1,
                        MenuId = 1,
                        Amount = 1,
                    }
                },
                OrderDate = DateTime.UtcNow,
                Seats = new System.Collections.Generic.List<OrderSeat>
                {
                    new OrderSeat
                    {
                        SeatId = 1,
                        MenuId = 1,
                        BeverageId = 1,
                        AllergyId = 1,
                    }
                },
                TourId = 1,
                TotalPrice = 1,
                Surprises = new System.Collections.Generic.List<OrderSurprise>(0)
            };

            var orderRepository = IoC.GetRequiredService<IOrderRepository>();
            await orderRepository.TryInsertOrderAsync(order);

            var process = IoC.GetRequiredService<ITourOrderProcess>();
            process.Reset();
            await process.SetContextAsync(order);
            await process.InitStateAsync();

            return order.Id;
        }
    }
}
