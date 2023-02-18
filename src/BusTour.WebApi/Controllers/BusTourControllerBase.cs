using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NLog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BusTour.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public abstract class BusTourControllerBase :ControllerBase
    {
        protected readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        protected readonly IActionContextAccessor _actionContextAccessor = IoC.GetRequiredService<IActionContextAccessor>();

        protected async Task<ActionResult<T>> RunCommandAsync<T>(IMediatorCommand<T, MediatorCommandResult<T>> Command)
        {
            MediatorCommandResult<T> result = null;
            await WrapAsync(async ()=>
            {
                result = await IoC.GetRequiredService<IMediator>()
                    ?.RunCommandAsync(Command);
            });

            if (!string.IsNullOrEmpty(result.ErrorMessage) || result.ErrorData != null)
            {
                return BadRequest(new { message = result.ErrorMessage, data = result.ErrorData });
            }
            else if (result.Result == null)
            {
                return NotFound();
            }

            return Ok(result.Result);
        }
        protected async Task<IActionResult> RunCommandNoTypedResultAsync<T>(IMediatorCommand<T, MediatorCommandResult<T>> Command)
        {
            MediatorCommandResult<T> result = null;
            await WrapAsync(async () =>
            {
                result = await IoC.GetRequiredService<IMediator>()
                    ?.RunCommandAsync(Command);
            });

            if (result.Result == null)
            {
                if (string.IsNullOrEmpty(result.ErrorMessage))
                {
                    return NotFound();
                }
                else
                {
                    return BadRequest();
                }
            }

            return Ok();
        }

        protected async Task<ActionResult<T>> RunCommandAsyncs<T>(IMediatorCommand command)
        {
            await WrapAsync(() => IoC.GetRequiredService<IMediator>()
                    ?.RunCommandAsync(command));

            return Ok();
        }

        protected Task WrapAsync(Func<Task> action)
        {
            try
            {
                return action?.Invoke();
            }
            catch (Exception e)
            {
                _logger.Error(e, $"{string.Join('/', _actionContextAccessor.ActionContext.RouteData.Values.Select(i => i.Value))} threw exception");
                throw;
            }
        }
    }
}