using System.Diagnostics;
using Infrastructure.Common.DI;
using Infrastructure.Common.Logging;
using Infrastructure.Web.Bundles.Helpers;
using Infrastructure.Web.Bundles.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Web.Bundles
{
    public abstract class BaseMvcController : Controller
    {
        protected readonly ILogger _logger;
        protected readonly bool _isProduction;
        protected readonly bool _isLocal;

        protected WebpackHelperService WebpackHelperService { get; }

        public BaseMvcController()
        {
            var env = IoC.GetRequiredService<IWebHostEnvironment>();
            WebpackHelperService = IoC.GetRequiredService<WebpackHelperService>();
            _logger = Log.For(GetType().Name);
            _isProduction = !string.Equals(env.EnvironmentName, "Development");
            _isLocal = env.IsDevelopment();
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.BundleMainJs = WebpackHelperService.GetBundlePath("main", "js");

            ViewBag.IsProduction = _isProduction;
            ViewBag.IsLocal = _isLocal;
            ViewBag.Path = $"{Request.Scheme}://{Request.Host}{Url.Content("~")}";

            base.OnActionExecuting(filterContext);
        }

        [HttpGet]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var ex = feature?.Error;
            if (ex != null)
            {
                _logger.LogError(ex, ex.Message);
            }
            else
            {
                _logger.LogError("Не удалось получить сведения об ошибке.");
            }

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
