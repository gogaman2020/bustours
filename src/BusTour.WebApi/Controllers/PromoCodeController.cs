using BusTour.AppServices.CommonConfigService;
using BusTour.AppServices.PromoCodes;
using BusTour.Data.Repositories.PromoCodes;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Infrastructure.Common.Json;
using Infrastructure.Db.Responses;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace BusTour.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [InjectAsSingleton]
    public class PromoCodeController : BusTourControllerBase
    {
        private readonly ICommonConfigService _commonConfigService;
        private readonly IPromoCodeRepository _promoCodeRepository;

        public PromoCodeController(ICommonConfigService commonConfigService)
        {
            _commonConfigService = commonConfigService;
            _promoCodeRepository = IoC.GetRequiredService<IPromoCodeRepository>();
        }

        [HttpPost]
        [Route("CreatePromoCode")]
        public async Task<ActionResult<PromoCode>> CreatePromoCode(PromoCodeAddCommand command)
        {
            return await RunCommandAsync(command);
        }

        [HttpGet]
        [Route("{seriesNumber}")]
        public async Task<ActionResult<PromoCode>> GetPromoCode(string seriesNumber)
        {
            return await _promoCodeRepository.GetByFilterAsync(new PromoCodeFilter { SeriesNumber = seriesNumber });
        }

        [HttpPost]
        [Route("get-with-validate")]
        public async Task<ActionResult<PromoCode>> GetValidePromoCode(ValidatePromoCodeCommand command)
        {
            return await RunCommandAsync(command);
        }

        [HttpGet]
        [Route("GetAmountOfDiscount")]
        public async Task<ActionResult<int[]>> GetAmountOfDiscount()
        {
            var config = await _commonConfigService.GetCommonConfigAsync();

            var amountOfDiscount = config.FirstOrDefault(x => x.Code == "promo_code_discount_amount").Value;

            return JsonConvert.DeserializeObject<int[]>(amountOfDiscount);
        }

        [HttpGet]
        [Route("GetPromoCodeList")]
        public async Task<ActionResult<DataSourceResponse<PromoCodeGridModel>>> GetPromoCodeList(string request)
        {
            var parsedRequest = request.FromJson<PromoCodeDataSourceRequest>();

            return await RunCommandAsync(new GetPromoCodeListCommand {Request = parsedRequest });
        }
        
        [HttpPut("UpdatePromoCode")]
        public async Task<ActionResult<PromoCode>> UpdatePromoCode(PromoCode promoCode)
        {
            return await RunCommandAsync(new UpdatePromoCodeCommand { PromoCode = promoCode });
        }
    }
}
