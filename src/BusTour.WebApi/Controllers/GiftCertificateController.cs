using BusTour.AppServices.GiftCertificates.Commands;
using BusTour.AppServices.GiftCertificates.Queries;
using BusTour.Data.Repositories.GiftCertificates;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusTour.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [InjectAsSingleton]
    public class GiftCertificateController : BusTourControllerBase
    {
        [HttpPost]
        [Route("AddCertificate")]
        public async Task<ActionResult<GiftCertificate>> AddCertificate(GiftCertificateAddCommand command)
        {
            return await RunCommandAsync(command);
        }

        [HttpGet]
        [Route("GetAmountVariants")]
        public async Task<ActionResult<List<GiftCertificateAmountVariant>>> GetAmountVariants()
        {
            return await RunCommandAsync(new GetAmountVariantsCommand());
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<GiftCertificate>> Get(string id)
        {
            return await RunCommandAsync(new GetGiftCertificateCommand { Id = id });
        }

        [HttpGet]
        [Route("Filter")]
        public async Task<ActionResult<List<GiftCertificate>>> Filter([FromQuery]string filter)
        {
            return await RunCommandAsync(new FilterGiftCertificatesCommand(JsonConvert.DeserializeObject<GiftCertificatesFilter>(filter)));
        }

        [HttpGet]
        [Route("GetStatusesTotals")]
        public async Task<ActionResult<List<GetGiftCertificatesStatusTotalsCommand.GiftCertificatesStatusTotals>>> GetStatusesTotals()
        {
            return await RunCommandAsync(new GetGiftCertificatesStatusTotalsCommand());
        }

        [HttpPost]
        [Route("SendCertificateOnEmail")]
        public async Task<ActionResult<GiftCertificate>> SendCertificateOnEmail(SendGiftCertificateOnEmailCommand command)
        {
            return await RunCommandAsync(command);
        }

        [HttpGet("GetCertificatePdf")]
        public async Task<ActionResult<byte[]>> GetCertificatePdf(int certificateId)
        {
            return await RunCommandAsync(new GetCertificatePdfCommand(certificateId));
        }
    }
}
