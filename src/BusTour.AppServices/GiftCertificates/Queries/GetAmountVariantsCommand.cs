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
    public class GetAmountVariantsCommand : HighLevelMediatorCommand<List<GiftCertificateAmountVariant>>
    {
        private readonly IGiftCertificateRepository _giftCertificateRepository;
        public GetAmountVariantsCommand()
        {
            _giftCertificateRepository = IoC.GetRequiredService<IGiftCertificateRepository>();
        }

        public override async Task<MediatorCommandResult<List<GiftCertificateAmountVariant>>> ExecuteAsync()
        {
            return Success(await _giftCertificateRepository.GetAmountVariantsAsync());
        }
    }
}
