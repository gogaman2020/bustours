using BusTour.Data.Repositories.PromoCodes;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Infrastructure.Db.Responses;
using Infrastructure.Mediator;
using System.Threading.Tasks;

namespace BusTour.AppServices.PromoCodes
{
    [InjectAsScoped]
    public class GetPromoCodeListCommand : HighLevelMediatorCommand<DataSourceResponse<PromoCodeGridModel>>
    {
        private readonly IPromoCodeRepository _promoCodeRepository;

        public GetPromoCodeListCommand()
        {
            _promoCodeRepository = IoC.GetRequiredService<IPromoCodeRepository>();
        }

        public PromoCodeDataSourceRequest Request { get; set; }

        public override async Task<MediatorCommandResult<DataSourceResponse<PromoCodeGridModel>>> ExecuteAsync()
        {
            var result = await _promoCodeRepository.GetGridByRequestAsync(Request);

            return Success(result);
        }
    }
}
