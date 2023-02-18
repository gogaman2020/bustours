using BusTour.AppServices.GiftCertificates.Commands;
using BusTour.AppServices.GiftCertificates.Queries;
using BusTour.Data.Repositories.GiftCertificates;
using BusTour.Data.Repositories.Tours;
using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusTour.Test
{
    [TestClass]
    public class GiftCertificateTest : UnitTestBase
    {
        [TestMethod]
        public async Task AddCertificate()
        {
            var mediator = IoC.GetRequiredService<IMediator>();

            var amountVariants = await IoC.GetRequiredService<IGiftCertificateRepository>().GetAmountVariantsAsync();
            var surprises = await IoC.GetRequiredService<ITourRepository>().GetSurprisesAsync();

            var command = new GiftCertificateAddCommand
            {
                Comment = "Add certificate",
                HasSurprises = true,
                CertificateSurprises = new List<GiftCertificateSurprise>
                {
                    new GiftCertificateSurprise { SurpriseId = surprises.First().Id, Quantity = 2 },
                    new GiftCertificateSurprise { SurpriseId = surprises.First().Id, Quantity = 3 },
                    new GiftCertificateSurprise { SurpriseId = surprises.First().Id, Quantity = 4 }
                },
                AmountVariantId = amountVariants[1].Id,
                Email = "gogaman@mail.ru"
            };

            await mediator.RunCommandAsync(command, async result =>
            {
                var certificate = result.Result;
                Assert.IsTrue(certificate.Client?.Id != null);
                Assert.IsTrue(result.ErrorMessage == null);
                Assert.AreEqual(command.CertificateSurprises.Count, certificate.CertificateSurprises.Where(x => x.CertificateId != default(int)).Count());
                Assert.AreEqual(amountVariants[1].Id, certificate.AmountVariant.Id);
                Assert.AreEqual(surprises.First().Id, certificate.CertificateSurprises.First().SurpriseId);
                Assert.AreEqual(command.CertificateSurprises.Sum(x => x.Quantity), certificate.CertificateSurprises.Sum(x => x.Quantity));
            });
        }

        [TestMethod]
        public async Task GetAmountVariants()
        {
            await IoC.GetRequiredService<IMediator>().RunCommandAsync(new GetAmountVariantsCommand(), async result =>
            {
                Assert.IsTrue(result.Result.Any());
            });
        }

        [TestMethod]
        public async Task CancelCertificate()
        {
            var mediator = IoC.GetRequiredService<IMediator>();

            var amountVariants = await IoC.GetRequiredService<IGiftCertificateRepository>().GetAmountVariantsAsync();

            var certificate = (await mediator.RunCommandAsync(new GiftCertificateAddCommand
            {
                Comment = "Cancel certificate",
                HasSurprises = false,
                CertificateSurprises = new List<GiftCertificateSurprise>(),
                AmountVariantId = amountVariants.First().Id,
            })).Result;

            certificate = (await mediator.RunCommandAsync(new GiftCertificateCancelCommand
            {
                Id = certificate.Id.ToString()
            })).Result;

            await mediator.RunCommandAsync(new FilterGiftCertificatesCommand(new GiftCertificatesFilter 
            {
                Statuses = new List<GiftCertificateStatus> 
                { 
                    GiftCertificateStatus.Сancelled 
                }
            }), async result =>
            {
                Assert.IsTrue(result.Result.Any(x => x.Id == certificate.Id));
            });
        }

        [TestMethod]
        public async Task FilterCertificate()
        {
            var mediator = IoC.GetRequiredService<IMediator>();

            var statuses = new List<GiftCertificateStatus>
            {
                GiftCertificateStatus.Сancelled,
                GiftCertificateStatus.Redeemed
            };

            await mediator.RunCommandAsync(new FilterGiftCertificatesCommand
            (
                new GiftCertificatesFilter
                {
                    Statuses = statuses
                }
            ), async result =>
            {
                Assert.IsTrue(result.Result.Any(x => statuses.Contains(x.Status)));
                Assert.IsTrue(!result.Result.Any(x => x.Status == GiftCertificateStatus.Active));
            });
        }
    }
}
