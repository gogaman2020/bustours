using BusTour.Scheduler.Helpers;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BusTour.Scheduler.Clients
{
    public class WebApiHttpClient : IDisposable
    {
        private readonly HttpClient _httpClient;

        public HttpClient HttpClient => _httpClient;
        public ILogger _logger = LogManager.GetCurrentClassLogger();

        public WebApiHttpClient(string baseUri)
        {
            if (string.IsNullOrEmpty(baseUri))
            {
                throw new ArgumentNullException("baseUri");
            }

            var _baseAdress = new Uri(baseUri);
            var _cookieContainer = new CookieContainer();
            var _httpClientHandler = new HttpClientHandler
            {
                CookieContainer = _cookieContainer
            };
            _httpClient = new HttpClient(_httpClientHandler) { BaseAddress = _baseAdress };

            if (_httpClient.BaseAddress.Scheme == Uri.UriSchemeHttps)
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol =
                    SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            }

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string> GetRawAsync(string methodUrl)
        {
            var response = await InvokeAsync(methodUrl, (client, url) =>
            {
                return client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            });
            var result = await GetRawResultAsync(response);
            return result;
        }

        public async Task<string> PostRawAsync(string methodUrl, string param)
        {
            var response = await InvokeAsync(methodUrl, (client, url) =>
            {
                var httpContent = new StringContent(param, Encoding.UTF8, "application/json");
                return client.PostAsync(url, httpContent);
            });
            var result = await GetRawResultAsync(response);
            return result;
        }

        public async Task<TResult> GetAsync<TResult>(string methodUrl, object args = null,
            Func<bool, int, string, bool> customResultHandler = null)
        {
            var requestUrl = methodUrl;
            if (args != null)
            {
                requestUrl += ToQueryString(args);
            }

            var response = await InvokeAsync(requestUrl, (client, url) =>
            {
                return client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            });

            var str = await GetRawResultAsync(response);
            if (customResultHandler?.Invoke(response.IsSuccessStatusCode, (int)response.StatusCode, str) ?? true)
            {
                var result = GetResult<TResult>(str);
                return result;
            }
            return default;
        }

        public async Task<TResult> PostAsync<TResult, TArg>(string methodUrl, TArg param,
            Func<bool, int, string, bool> customResultHandler = null)
        {
            var response = await InvokeAsync(methodUrl, (client, url) =>
            {
                var httpContent = new StringContent(param.ToJson(), Encoding.UTF8, "application/json");
                return client.PostAsync(url, httpContent);
            });

            var str = await GetRawResultAsync(response);
            if (customResultHandler?.Invoke(response.IsSuccessStatusCode, (int)response.StatusCode, str) ?? true)
            {
                var result = GetResult<TResult>(str);
                return result;
            }
            return default;
        }

        public Task PostAsync<TArg>(string methodUrl, TArg param)
        {
            return InvokeAsync(methodUrl, (client, url) =>
            {
                var httpContent = new StringContent(param.ToJson(), Encoding.UTF8, "application/json");
                return client.PostAsync(url, httpContent);
            });
        }

        public async Task<TResult> PutAsync<TResult, TArg>(string methodUrl, TArg param,
            Func<bool, int, string, bool> customResultHandler = null)
        {
            var response = await InvokeAsync(methodUrl, (client, url) =>
            {
                var httpContent = new StringContent(param.ToJson(), Encoding.UTF8, "application/json");
                return client.PutAsync(url, httpContent);
            });

            var str = await GetRawResultAsync(response);
            if (customResultHandler?.Invoke(response.IsSuccessStatusCode, (int)response.StatusCode, str) ?? true)
            {
                var result = GetResult<TResult>(str);
                return result;
            }
            return default;
        }

        public Task PutAsync<TArg>(string methodUrl, TArg param)
        {
            return InvokeAsync(methodUrl, (client, url) =>
            {
                var httpContent = new StringContent(param.ToJson(), Encoding.UTF8, "application/json");
                return client.PutAsync(url, httpContent);
            });
        }

        private async Task<HttpResponseMessage> InvokeAsync(string methodUrl, Func<HttpClient, string, Task<HttpResponseMessage>> invoker)
        {
            //_logger.Debug("InvokeAsync");
            //_logger.Debug($"{_httpClient.BaseAddress} + {methodUrl}");
            //_logger.Debug(response.StatusCode);

            var response = await invoker(_httpClient, FormattedUrl(methodUrl));

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var message = await GetRawResultAsync(response);
                throw new ApplicationException(message);
            }

            return response;
        }

        #region get result

        private TResult GetResult<TResult>(string responseString)
        {
            var result = responseString.FromJson<TResult>();
            return result;
        }

        private async Task<string> GetRawResultAsync(HttpResponseMessage response)
        {
            using (response)
            {
                using (var content = response.Content)
                {
                    var responseString = await content.ReadAsStringAsync();
                    return responseString;
                }
            }
        }
        #endregion

        #region methodUrl

        private static string FormattedUrl(string methodUrl)
        {
            return methodUrl.TrimStart('/');
        }

        #endregion

        #region arguments
        private string ToQueryString(object args)
        {
            if (args == null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("?");
            bool first = true;
            foreach (var property in args.GetType().GetProperties())
            {
                ToQueryArgument(sb, args, property, ref first);
            }

            return sb.ToString();
        }

        private void ToQueryArgument(StringBuilder sb, object args, PropertyInfo property, ref bool first)
        {
            if (property.PropertyType.IsConstructedGenericType &&
                property.PropertyType.GetGenericTypeDefinition() == typeof(List<>) ||
                property.PropertyType.IsArray)
            {
                var types = property.PropertyType.IsArray
                    ? new[] { property.PropertyType.GetElementType() }
                    : property.PropertyType.GenericTypeArguments;

                var listToQueryArguments = typeof(WebApiHttpClient)
                    .GetMethod(nameof(ListToQueryArguments), BindingFlags.NonPublic | BindingFlags.Static)
                    .MakeGenericMethod(types);

                first = (bool)listToQueryArguments.Invoke(this,
                    new[] { sb, HttpUtility.UrlEncode(property.Name.ToLower()), property.GetValue(args), first });
                return;
            }

            var value = property.GetValue(args);
            if (value != null)
            {
                if (!first)
                {
                    sb.Append("&");
                }

                sb.Append(HttpUtility.UrlEncode(property.Name.ToLower()))
                    .Append("=")
                    .Append(HttpUtility.UrlEncode(property.GetValue(args).ToString()));
                first = false;
            }
        }

        private static bool ListToQueryArguments<T>(StringBuilder sb, string name, IEnumerable<T> values, bool first)
        {
            foreach (var value in values)
            {
                if (!first)
                {
                    sb.Append("&");
                }
                first = false;

                sb.Append(name)
                    .Append("=")
                    .Append(HttpUtility.UrlEncode(value.ToString()));
            }
            return first;
        }

        #endregion

        #region Disposable

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _httpClient.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
