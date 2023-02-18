using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Authentication;
using System.Threading;
using Infrastructure.Common.DI;
using Infrastructure.Security.SecurityTokens;

namespace Infrastructure.Common.HttpClients
{
    /// <summary>
    /// Интерфейс для генереации HttpClient для клиентов приложений с их кучей особенностей
    /// </summary>
    public interface IWebApiHttpClientFactory : IHttpClientFactory
    {
    }
    
    [InjectAsSingleton(typeof(IWebApiHttpClientFactory), typeof(ISecurityTokensEventListener))]
    public class WebApiHttpClientFactory : IWebApiHttpClientFactory
    {
        private readonly AsyncLocal<ConcurrentDictionary<string, SocketsHttpHandler>> _handlersCache = 
            new AsyncLocal<ConcurrentDictionary<string, SocketsHttpHandler>>();

        public HttpClient CreateClient(string name)
        {
            var handler = CreateHandler(name);
            var httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient;
        }

        private SocketsHttpHandler CreateHandler(string name)
        {
            var cache = _handlersCache.Value ??= new ConcurrentDictionary<string, SocketsHttpHandler>();
            var handler = cache.GetOrAdd(name, key => CreateDefaultHandler());
            return handler;
        }

        private SocketsHttpHandler CreateDefaultHandler()
        {
            var handler = new SocketsHttpHandler()
            {
                UseCookies = false,
                //CookieContainer = new CookieContainer(),
                ConnectTimeout = TimeSpan.FromSeconds(1),
                PooledConnectionLifetime = TimeSpan.FromSeconds(10),
                PooledConnectionIdleTimeout = TimeSpan.FromSeconds(10),
                Expect100ContinueTimeout = TimeSpan.FromSeconds(10),
                SslOptions = new SslClientAuthenticationOptions
                {
                    ApplicationProtocols = new List<SslApplicationProtocol>
                    {
                        SslApplicationProtocol.Http11,
                        SslApplicationProtocol.Http2,
                    },
                    EnabledSslProtocols = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12,
                },
            };
            return handler;
        }
    }
}