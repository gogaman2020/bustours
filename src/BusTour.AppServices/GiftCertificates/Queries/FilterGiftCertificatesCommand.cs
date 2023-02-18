using BusTour.Data.Repositories.GiftCertificates;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusTour.AppServices.GiftCertificates.Queries
{
    public class FilterGiftCertificatesCommand : HighLevelMediatorCommand<List<GiftCertificate>>
    {
        private GiftCertificatesFilter _filter { get; }

        public FilterGiftCertificatesCommand(GiftCertificatesFilter filter) : base()
        {
            _filter = filter;
        }

        public override async Task<MediatorCommandResult<List<GiftCertificate>>> ExecuteAsync()
        {
            return Success(await IoC.GetRequiredService<IGiftCertificateRepository>().FilterAsync(_filter));
        }
    }
}
