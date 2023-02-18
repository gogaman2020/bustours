using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Web.Security
{
    public static class TokenHelper
    {
        public static string OtcTokenKey => "OtcToken";

        public static string OtcTokenEndDateTimeKey => "OtcTokenEndDateTime";

        public static string MsOwinContext => "MS_OwinContext";

        public static string AuthorizationHeader => "Authorization";

        /// <summary>
        /// Возвращает текущий токен из HttpContext.
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentTokenFromContext(HttpContext httpContext, string cookieName)
        {
            return GetSecurityTokenFromCookies(httpContext, cookieName);
        }

        /// <summary>
        /// Получить текущий токен.
        /// </summary>
        /// <param name="actionContext">Контекст действия.</param>
        /// <returns></returns>
        public static string GetCurrentToken(IDictionary<string, object> properties)
        {
            var otcToken = string.Empty;

            if (properties == null)
            {
                return string.Empty;
            }

            //var owinContext = properties[TokenHelper.MsOwinContext] as Microsoft.Owin.OwinContext;
            //if (owinContext != null)
            //{
            //    otcToken = owinContext.Get<string>(TokenHelper.OtcTokenKey);
            //}

            return otcToken;
        }

        public static string GetTokenFromHeader(string header)
        {
            return string.IsNullOrEmpty(header) 
                ? string.Empty
                : header.Replace("Bearer ", string.Empty);
        }

        public static string GetTokenFromHeader(HttpContext httpContext)
        {
            var header = httpContext.Request.Headers[TokenHelper.AuthorizationHeader]; 
            return string.IsNullOrEmpty(header) 
                ? string.Empty
                : GetTokenFromHeader(header);
        }

        public static string GetTokenFromHeader(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext != null)
            {
                return GetTokenFromHeader(httpContextAccessor.HttpContext);
            }

            return string.Empty;
        }

        /// <summary>
        /// Взять токен из coockies.
        /// </summary>
        /// <returns>Токен безопасности.</returns>
        private static string GetSecurityTokenFromCookies(HttpContext httpContext, string cookieName)
        {
            var cookie = httpContext?.Request.Cookies[cookieName];
            return cookie ?? string.Empty;
        }
    }
}
