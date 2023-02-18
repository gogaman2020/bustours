using System.Linq;
using Infrastructure.Common.DI;
using Infrastructure.Queue.InfrastructurePlugin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Web.Queue
{
    /// <summary>
    /// todo
    /// </summary>
    [ApiController]
    [Route("api/infra/[controller]")]
    [InjectAsSingleton]
    public class QueueController
    {
        public QueueController(ILogger<QueueController> logger)
        {
        }

        [HttpGet]
        [Route("subscribers")]
        public ActionResult SubscribersAsync()
        {
            var result = QueueInfrastructurePlugin.Subscribers
                .Select(t =>
                    new
                    {
                        Name = t.Name,
                        Message = t.GetInterfaces()
                            .Where(i=> i.IsGenericType && i.Name.StartsWith("ISubscriber"))
                            .Select(i=>i.GenericTypeArguments[0].Name),
                    })
                .ToArray();
            return new OkObjectResult(result);
        }

        [HttpGet]
        [Route("subscribers/{name}")]
        public ActionResult MetaInfoAsync(string name)
        {
            return new OkResult();
        }
        [HttpPost]
        [Route("subscribers/{name}/start")]
        public ActionResult StartAsync(string name)
        {
            return new OkResult();
        }
        [HttpPost]
        [Route("subscribers/{name}/stop")]
        public ActionResult StopAsync(string name)
        {
            return new OkResult();
        }
    }
}