using BusTour.Data.Repositories.GiftCertificates;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BusTour.Domain.Enums;
using System.Linq;

namespace BusTour.AppServices.GiftCertificates.Queries
{
    public class GetGiftCertificatesStatusTotalsCommand : HighLevelMediatorCommand<List<GetGiftCertificatesStatusTotalsCommand.GiftCertificatesStatusTotals>>
    {
        public override async Task<MediatorCommandResult<List<GiftCertificatesStatusTotals>>> ExecuteAsync()
        {
            var certificates = await IoC.GetRequiredService<IGiftCertificateRepository>().FilterAsync(new GiftCertificatesFilter());
            return Success(certificates.GroupBy(x => x.Status).Select(x => new GiftCertificatesStatusTotals
            {
                Status = x.Key,
                Count = x.Count(),
                Amount = x.Sum(z => z.Amount ?? 0),
                Balance = x.Sum(z => z.Balance)
            }).OrderBy(x => x.Status).ToList());
        }

        public class GiftCertificatesStatusTotals
        {
            public GiftCertificateStatus Status { get; set; }

            public int Count { get; set; }

            public decimal Amount { get; set; }

            public decimal Balance { get; set; }
        }
    }
}
