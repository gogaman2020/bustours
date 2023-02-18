using System;
using System.Collections.Generic;
using System.Threading;
using Infrastructure.Common.Configs;
using Infrastructure.Common.DI;
using Infrastructure.Common.Logging;
using Infrastructure.Security.Configs;
using Infrastructure.Security.Helpers;
using Infrastructure.Security.Models;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Security.SecurityTokens
{
    [InjectAsSingleton]
    public class SecurityTokensProvider : ISecurityTokensSetter
    {
        private class TokensHolder
        {
            private UserInfo _userInfo;
            public string Token { get; set; }
            public string TokenV2 { get; set; }
            public UserInfo UserInfo => _userInfo ??= Parse();
            public int CacheId { get; set; }

            private UserInfo Parse()
            {
                var key = Config.Get<MachineKeyDto>();

                var (ex, userInfo) = Decrypt(key, Token);
                if (ex == null)
                {
                    return userInfo;
                }

                (ex, userInfo) = Decrypt(key, TokenV2);
                if (ex != null)
                {
                    throw ex;
                }

                return userInfo;
            }

            private (Exception e, UserInfo token) Decrypt(MachineKeyDto machineKey, string securityTokenString)
            {
                try
                {
                    var securityToken = SecurityTokenUtility.Deserialize(machineKey, securityTokenString);
                    return (null, securityToken);
                }
                catch (Exception e)
                {
                    return (e, null);
                }
            }
        }

        private static readonly AsyncLocal<TokensHolder> _tokens = new AsyncLocal<TokensHolder>();
        private readonly IEnumerable<ISecurityTokensEventListener> _listeners;
        private readonly ILogger<SecurityTokensProvider> _logger;

        public SecurityTokensProvider(IConfig<SecurityConfig> config)
        {
            SecurityTokenName = config.Value.CookieNamePrefix + "SecurityTokenKey";
            SecurityTokenV2Name = config.Value.CookieNamePrefix + "SecurityTokenKeyV2";
            _listeners = IoC.GetService<IEnumerable<ISecurityTokensEventListener>>();
            _logger = Log.For<SecurityTokensProvider>();
        }

        public string SecurityTokenName { get; }
        public string SecurityToken => _tokens.Value?.Token;
        public string SecurityTokenV2Name { get; }
        public string SecurityTokenV2 => _tokens.Value?.TokenV2;

        public UserInfo UserInfo => _tokens.Value?.UserInfo;
        public int CacheId => _tokens.Value?.CacheId ?? default;

        public void SetTokens(string token, string tokenv2)
        {
            var newCacheId = CacheId + 1;
            _tokens.Value = new TokensHolder
            {
                Token = token,
                TokenV2 = tokenv2,
                CacheId = newCacheId,
            };
            NotifyChanged();
        }

        private void NotifyChanged()
        {
            foreach (var securityTokensEventListener in _listeners)
            {
                try
                {
                    securityTokensEventListener.TokensChanged(this);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error handle TokensChanged");
                }
            }
        }
    }
}