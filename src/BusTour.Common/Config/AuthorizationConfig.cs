using Infrastructure.Common.Configs;
using System.Collections.Generic;

namespace BusTour.Common.Config
{
    [Config]
    public class AuthorizationConfig
    {
        public string Secret { get; set; }

        public Dictionary<string, short> Expiration { get; set; }
    }
}