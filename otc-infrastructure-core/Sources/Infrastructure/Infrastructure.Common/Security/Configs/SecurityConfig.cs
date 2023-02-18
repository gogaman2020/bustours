using Infrastructure.Common.Configs;

namespace Infrastructure.Security.Configs
{
    [Config]
    public class SecurityConfig
    {
        public string CookieNamePrefix { get; set; }

        public string SecurityTokenKey { get; set; }
    }
}
