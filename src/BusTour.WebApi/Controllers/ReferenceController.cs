using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using BusTour.AppServices.ReferenceService;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using NLog;

namespace BusTour.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [InjectAsSingleton]
    public class ReferenceController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IReferenceService _referenceService;

        public ReferenceController(IActionContextAccessor actionContextAccessor, IReferenceService referenceService)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _actionContextAccessor = actionContextAccessor;
            _referenceService = referenceService;
        }

        [HttpGet]
        [Route("GetLanguages")]
        public async Task<IActionResult> GetLanguages()
        {
            try
            {
                var languages = await _referenceService.GetLanguagesAsync();
                
                return Ok(languages);
            }
            catch (Exception e)
            {
                _logger.Error(e, $"{String.Join('/', _actionContextAccessor.ActionContext.RouteData.Values.Select(i => i.Value))} threw exception");
                return StatusCode((int)HttpStatusCode.InternalServerError,  e.InnerException != null ? e.InnerException.Message : e.Message);
            }
        }

        [HttpGet]
        [Route("GetDomainEnums")]
        public async Task<Dictionary<string, List<SelectListItem>>> GetDomainEnums()
        {
            var enums = new Dictionary<string, List<SelectListItem>>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetName().Name == "BusTour.Domain"))
            {
                foreach (var enumType in assembly.GetTypes().Where(x => x.Namespace == "BusTour.Domain.Enums" && x.IsEnum))
                {
                    enums.Add(enumType.Name, new List<SelectListItem>());

                    foreach (var el in Enum.GetValues(enumType))
                    {
                        enums[enumType.Name].Add(new SelectListItem(el.ToString(), ((int)el).ToString()));
                    }
                }
            }

            return enums;
        }
    }
}