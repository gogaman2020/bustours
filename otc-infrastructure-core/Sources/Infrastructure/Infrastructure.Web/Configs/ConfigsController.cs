using System.Linq;
using Infrastructure.Common.Configs;
using Infrastructure.Common.DI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Web.Configs
{
    /// <summary>
    /// todo
    /// </summary>
    [ApiController]
    [Route("api/infra/[controller]")]
    [InjectAsSingleton]
    public class ConfigsController
    {
        public ConfigsController(ILogger<ConfigsController> logger)
        {
        }

        [HttpGet]
        [Route("list")]
        public ActionResult ListAsync()
        {
            var result = ConfigInfrastructurePlugins.ConfigTypes
                .Select(t => t.Name)
                .ToArray();
            return new OkObjectResult(result);
        } 

        [HttpGet]
        [Route("{name}")]
        public ActionResult GetAsync(string name)
        {
            //var result = ConfigInfrastructurePlugins.ConfigTypes.Select(t => t.Name).ToArray();
            // Config.Get<>()
            //     ConfigExtension.FillFromConfig();
            return new OkResult();
        } 

        [HttpPost]
        [Route("{name}")]
        public ActionResult SetAsync(string name, [FromBody] object obj)
        {
            var result = ConfigInfrastructurePlugins.ConfigTypes.Select(t => t.Name).ToArray();
            return new OkResult();
        } 
    }
}