using BusTour.Data.Repositories.PromoCodes;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Infrastructure.Common.Extensions;
using Infrastructure.Db.Responses;
using Infrastructure.Mediator;
using System.Threading.Tasks;

namespace BusTour.AppServices.PromoCodes
{
    [InjectAsSingleton]
    public class UpdatePromoCodeCommand : HighLevelMediatorCommand<PromoCode>
    {
        private readonly IPromoCodeRepository _promoCodeRepository;

        public UpdatePromoCodeCommand()
        {
            _promoCodeRepository = IoC.GetRequiredService<IPromoCodeRepository>();
        }

        public PromoCode PromoCode { get; set; }

        public override async Task<MediatorCommandResult<PromoCode>> ExecuteAsync()
        {
            var promoCode = await _promoCodeRepository.GetAsync(PromoCode.Id);

            PromoCode.DateStart = PromoCode.DateStart.HasValue
                ? PromoCode.DateStart.ToString().ToUtcDateTime()
                : null;

            PromoCode.DateEnd = PromoCode.DateEnd.HasValue
                ? PromoCode.DateEnd.ToString().ToUtcDateTime()
                : null;

            promoCode.DateStart = PromoCode.DateStart;
            promoCode.DateEnd = PromoCode.DateEnd;
            promoCode.NumberOfPromocodes = PromoCode.NumberOfPromocodes;
            promoCode.IsActive = PromoCode.IsActive;

            await _promoCodeRepository.SaveOrUpdateAsync(promoCode);

            promoCode = await _promoCodeRepository.GetAsync(PromoCode.Id);

            return Success(promoCode);
        }
    }
}
