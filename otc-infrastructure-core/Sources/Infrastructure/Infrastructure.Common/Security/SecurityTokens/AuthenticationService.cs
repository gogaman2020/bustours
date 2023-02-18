using System;
using System.Threading.Tasks;
using Infrastructure.Common.DI;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Security.SecurityTokens
{
    [InjectAsSingleton]
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthClient _authClient;
        private readonly ISecurityTokensSetter _setter;
        private readonly IMemoryCache _memoryCache;
        private readonly TimeSpan _cacheTime = new TimeSpan(0, 15, 0); //15min

        public AuthenticationService(IAuthClient authClient,
            ISecurityTokensSetter setter,
            IMemoryCache memoryCache)
        {
            _authClient = authClient;
            _setter = setter;
            _memoryCache = memoryCache;
        }

        public async Task RunAsSysUserAsync(string sysuser, int organizationId, int? platformId,
            Func<IServiceProvider, Task> func)
        {
            var key = $"{sysuser}_{organizationId}_{platformId ?? 0}";
            if (_memoryCache.TryGetValue(key, out var data) && data != null)
            {
                var tuple = (Tuple<string, string>) data;
                await RunAsync(tuple.Item1, tuple.Item2, func);
                return;
            }

            if (IoC.IsInScope)
            {
                await AddToCacheAsync(key, sysuser, organizationId, platformId, func);
            }
            else
            {
                await IoC.RunInNewScopeAsync(async () => await AddToCacheAsync(key, sysuser, organizationId, platformId, func));
            }
        }

        private async Task AddToCacheAsync(string key, string sysuser, int organizationId, int? platformId,
            Func<IServiceProvider, Task> func)
        {
            var (hasValue, token1, token2) = await GetAsync(sysuser, organizationId, platformId);
            if (hasValue)
            {
                _memoryCache.Set(key, new Tuple<string, string>(token1, token2), _cacheTime);
                await RunAsync(token1, token2, func);
            }
        }

        private async Task RunAsync(string token1, string token2, Func<IServiceProvider, Task> func)
        {
            _setter.SetTokens(token1, token2);
            await IoC.RunInNewScopeAsync(func);
        }

        private async Task<(bool, string, string)> GetAsync(string sysuser, int organizationId, int? platformId)
        {
            var response = await _authClient.SystemUserAuthenticateAsync(sysuser, organizationId, platformId);
            if (response.IsSuccess)
            {
                return (true, response.SecurityToken, response.SecurityTokenV2);
            }

            return (false, null, null);
        }
    }
}