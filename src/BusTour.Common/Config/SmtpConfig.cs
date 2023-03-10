using Infrastructure.Common.Configs;

namespace BusTour.Common.Config
{
    [Config]
    public class SmtpConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string From { get; set; }
    }
}
