using System.Text;
using Infrastructure.Web.Bundles.Helpers;
using Infrastructure.Web.Bundles.Models;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Web.Bundles
{
    public abstract class BaseBundlesController : BaseMvcController
    {
        [HttpGet]
        public virtual IActionResult Index()
        {
            return Ok();
        }

        [HttpGet]
        public ActionResult Get(string entryName)
        {
            var result = GetBundles(entryName);
            return Json(result);
        }

        public BundleModel GetBundles(string entryName)
        {
            WebpackHelperService.BundleResult bundleJs = WebpackHelperService.GetBundlePathSafe(entryName, "js");
            WebpackHelperService.BundleResult bundleCss = null;
            WebpackHelperService.BundleResult bundleJsCommon = null;
            WebpackHelperService.BundleResult bundleJsChunk = null;
            WebpackHelperService.BundleResult bundleCssCommon = null;
            WebpackHelperService.BundleResult bundleCssChunk = null;

            if (_isProduction)
            {
                bundleJsCommon = WebpackHelperService.GetBundlePathSafe("chunk-common", "js");
                bundleJsChunk = WebpackHelperService.GetBundlePathSafe("chunk-vendors", "js");
                bundleCssCommon = WebpackHelperService.GetBundlePathSafe("chunk-common", "css");
                bundleCssChunk = WebpackHelperService.GetBundlePathSafe("chunk-vendors", "css");
                bundleCss = WebpackHelperService.GetBundlePathSafe(entryName, "css");
            }

            var errorResult = GetError(bundleJs, bundleCss, bundleJsCommon, bundleJsChunk, bundleCssCommon, bundleCssChunk);

            return new BundleModel
            {
                IsSuccess = true,
                BundleCss = bundleCss?.Path,
                BundleJs = bundleJs?.Path,
                BundleJsCommon = bundleJsCommon?.Path,
                BundleJsChunk = bundleJsChunk?.Path,
                BundleCssCommon = bundleCssCommon?.Path,
                BundleCssChunk = bundleCssChunk?.Path,
                Path = ViewBag.Path,
                ErrorMessage = string.IsNullOrEmpty(errorResult)
                    ? string.Empty
                    : $"Произошла одна или несколько ошибок:\r\n{errorResult}"
            };
        }

        protected string GetError(params WebpackHelperService.BundleResult[] bundleResults)
        {
            if (bundleResults == null)
            {
                return null;
            }

            var errorMessage = new StringBuilder();
            
            foreach(var bundleResult in bundleResults)
            {
                if (bundleResult != null && !string.IsNullOrEmpty(bundleResult.Error))
                {
                    errorMessage.AppendLine(bundleResult.Error);
                }
            }

            return errorMessage.ToString();
        }
    }
}