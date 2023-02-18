using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace Infrastructure.Common.Logging
{
    public static class LogExtensions
    {
        public static ILoggingBuilder AutoConfigure(this ILoggingBuilder builder)
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddNLog();
            return builder;
        }
    }
}