using Infrastructure.Common.Configs;

namespace Infrastructure.Queue.Configs
{
    [Config("RabbitMqConfig")]
    public class ConnectionSettings
    {
        public string Host { get; set; } = "127.0.0.1";
        public string VirtualHost { get; set; } = "/";
        public string User { get; set; } = "guest";
        public string Password { get; set; } = "guest";
    }
}
