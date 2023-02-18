using Infrastructure.Common.Configs;

namespace Infrastructure.Common.AppInsights
{
    [Config]
    public class ApplicationInsightsSettings
    {
        public string Key { get; set; }
        public bool EnablePerformanceCounter { get; set; }
    }
}
