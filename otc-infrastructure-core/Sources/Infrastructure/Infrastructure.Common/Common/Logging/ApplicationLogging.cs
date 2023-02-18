using Microsoft.Extensions.Logging;

namespace Infrastructure.Common.Logging
{
    public static class ApplicationLogging
    {
        public static ILoggerFactory LoggerFactory { get; set; }
        public static ILogger<T> CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
        public static ILogger CreateLogger(string categoryName) => LoggerFactory.CreateLogger(categoryName);
    }

    public static class Log
    {
        public static ILogger<T> For<T>() => ApplicationLogging.LoggerFactory.CreateLogger<T>();
        public static ILogger For(string categoryName) => ApplicationLogging.LoggerFactory.CreateLogger(categoryName);
    }
}
