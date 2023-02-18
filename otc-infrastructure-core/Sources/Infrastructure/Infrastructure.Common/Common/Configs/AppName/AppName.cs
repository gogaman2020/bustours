using System;
using Infrastructure.Common.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.Common.Configs.AppName
{
    public static class AppName
    {
        private static readonly Lazy<string> LazyValue = new Lazy<string>(() => 
            Config.Get<AppNameConfig>()?.Name
            ?? throw new UndefinedAppNameException());

        public static string Name => LazyValue.Value;
        
        public static string GetNameFromConfig(IConfiguration configuration)
        {
            var sectionName = ConfigExtension.GetConfigName(typeof(AppNameConfig));
            var section = configuration.GetSection(sectionName);
            return section?[nameof(AppNameConfig.Name)] ?? throw new UndefinedAppNameException();
        }
    }
}