using Infrastructure.Common.DI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Infrastructure.Web.Bundles.Helpers
{
    [InjectAsSingleton]
    public class WebpackHelperService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public WebpackHelperService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public bool IsProduction => !string.Equals(_webHostEnvironment.EnvironmentName, "Development");
        public bool IsLocal => _webHostEnvironment.IsDevelopment();

        public string GetBundlePath(string bundleName, string bundleType)
        {
            var bandleFileName = GetBundleFileName("/bundles/Versioned/webpack.assets.json", bundleName, bundleType);
            return $"/bundles/Versioned/{bandleFileName}";
        }

        public BundleResult GetBundlePathSafe(string bundleName, string bundleType)
        {
            try
            {
                var bandleFileName = GetBundleFileName("/bundles/Versioned/webpack.assets.json", bundleName, bundleType);
                if (string.IsNullOrEmpty(bandleFileName))
                {
                    throw new Exception($"Не найден бандл {bundleName}.{bundleType}");
                }

                return new BundleResult($"/bundles/Versioned/{bandleFileName}");
            }
            catch (Exception ex)
            {
                return new BundleResult(null, ex.Message);
            }
        }

        public string GetBundlePath(string path, string assetFileName, string bundleName, string bundleType)
        {
            var bandleFileName = GetBundleFileName(path, assetFileName, bundleName, bundleType);

            if (string.IsNullOrEmpty(bandleFileName))
                return null;

            return $"{path}{bandleFileName}";
        }

        public string GetBundleFileName(string path, string assetFileName, string bundleName, string bundleType)
        {
            return GetBundleFileName($"{path}/{assetFileName}", bundleName, bundleType);
        }

        public string GetBundleFileName(string pathWithAssetFileName, string bundleName, string bundleType)
        {
            var applicationBasePath =  _webHostEnvironment.WebRootPath;
            string webpackAssetsJsonFilePath = $"{applicationBasePath}{pathWithAssetFileName}";

            using (StreamReader webpackAssetsFile = File.OpenText(webpackAssetsJsonFilePath))
            {
                using (JsonTextReader webpackAssetsReader = new JsonTextReader(webpackAssetsFile))
                {
                    var webpackAssetsJson = (JObject)JToken.ReadFrom(webpackAssetsReader);
                    var bandleFileName = webpackAssetsJson.SelectToken(bundleName).Value<string>(bundleType);
                    return bandleFileName;
                }
            }
        }

        public class BundleResult
        {
            public BundleResult(string path, string error = null)
            {
                Path = path;
                Error = error;
            }

            public string Path { get; set; }

            public string Error { get; set; }
        }
    }
}
