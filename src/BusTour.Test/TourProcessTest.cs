using BusTour.AppServices.TourProcess;
using BusTour.AppServices.TourProcess.Args;
using BusTour.AppServices.TourProcess.Commands;
using BusTour.AppServices.TourProcess.Steps;
using BusTour.Data.Repositories.Tours;
using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using BusTour.Domain.Models.Auth;
using Infrastructure.Common.DI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace BusTour.Test
{
    [TestClass]
    public class TourProcessTest: UnitTestBase
    {
        [TestMethod]
        public async Task ProcessOrderCreateAndCancelAsync()
        {
            SetContext(1, Role.Crew);
            var orderId = await InitTourAsync(TourType.Regular);
            var process = IoC.GetRequiredService<ITourProcess>();

            process.Reset();
            await process.SetContextAsync(orderId);
            Assert.IsTrue(process.CurrentStepName == nameof(TourDraftStep));

            await process.SendCommandAsync(TourStepCommand.Active);
            process.Reset();
            await process.SetContextAsync(orderId);
            Assert.IsTrue(process.CurrentStepName == nameof(TourActiveStep));

            await process.SendCommandAsync(TourStepCommand.CancelRequest);
            process.Reset();
            await process.SetContextAsync(orderId);
            Assert.IsTrue(process.CurrentStepName == nameof(TourCancelRequestStep));

            await process.SendCommandAsync(TourStepCommand.Cancel);
            process.Reset();
            await process.SetContextAsync(orderId);
            Assert.IsTrue(process.CurrentStepName == nameof(TourCanceledStep));

            await process.SendCommandAsync(TourStepCommand.Active);
            process.Reset();
            await process.SetContextAsync(orderId);
            Assert.IsTrue(process.CurrentStepName == nameof(TourActiveStep));
        }

        [TestMethod]
        public async Task AdminProcessServiceAndDeleteAsync()
        {
            SetContext(1, Role.Administrator);
            var orderId = await InitTourAsync(TourType.Service);
            var process = IoC.GetRequiredService<ITourProcess>();

            process.Reset();
            await process.SetContextAsync(orderId);
            Assert.IsTrue(process.CurrentStepName == nameof(TourDraftStep));

            await process.SendCommandAsync(TourStepCommand.Active);
            process.Reset();
            await process.SetContextAsync(orderId);
            Assert.IsTrue(process.CurrentStepName == nameof(TourActiveStep));

            await process.SendCommandAsync(TourStepCommand.Delete);
            process.Reset();
            await process.SetContextAsync(orderId);
            Assert.IsTrue(process.CurrentStepName == nameof(TourDeletedStep));
        }

        [TestMethod]
        public async Task AdminProcessNonServiceAndDeleteAsync()
        {
            SetContext(1, Role.Administrator);
            var orderId = await InitTourAsync(TourType.PrivateHire);
            var process = IoC.GetRequiredService<ITourProcess>();

            process.Reset();
            await process.SetContextAsync(orderId);
            Assert.IsTrue(process.CurrentStepName == nameof(TourDraftStep));

            await process.SendCommandAsync(TourStepCommand.Active);
            process.Reset();
            await process.SetContextAsync(orderId);
            Assert.IsTrue(process.CurrentStepName == nameof(TourActiveStep));

            await process.SendCommandAsync(TourStepCommand.Delete);
            process.Reset();
            await process.SetContextAsync(orderId);
            Assert.IsTrue(process.CurrentStepName == nameof(TourDeletedStep));
        }

        [TestMethod]
        public async Task AdminProcessCreateAndDeleteAsync()
        {
            SetContext(1, Role.Administrator);
            var orderId = await InitTourAsync(TourType.Regular);
            var process = IoC.GetRequiredService<ITourProcess>();

            process.Reset();
            await process.SetContextAsync(orderId);
            Assert.IsTrue(process.CurrentStepName == nameof(TourDraftStep));

            await process.SendCommandAsync(TourStepCommand.Active);
            process.Reset();
            await process.SetContextAsync(orderId);
            Assert.IsTrue(process.CurrentStepName == nameof(TourActiveStep));

            await process.SendCommandAsync(TourStepCommand.Delete);
            process.Reset();
            await process.SetContextAsync(orderId);
            Assert.IsTrue(process.CurrentStepName == nameof(TourDeletedStep));
        }

        protected override void BeforeTestStarted()
        {
        }

        protected override void AfterTestFinished()
        {
        }

        private async Task<int> InitTourAsync(TourType type)
        {
            var tour = new Tour
            {
                BusId = 1,
                RouteId = 1,
                Type = type,
                Departure = DateTime.UtcNow
            };

            var tourRepository = IoC.GetRequiredService<ITourRepository>();
            await tourRepository.SaveOrUpdateAsync(tour);
            return tour.Id;
        }
    }
}
