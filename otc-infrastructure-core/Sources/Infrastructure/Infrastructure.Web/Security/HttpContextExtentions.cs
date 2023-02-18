using System;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Web.Security
{
    public static class HttpContextExtentions
    {
        public static T GetCookie<T>(this HttpContext context, string key, Func<T> getAction = null)
        {
            var res = context.Items[key];
            if (res == null && getAction != null)
            {
                res = getAction();
                context.Items[key] = res;
            }

            return (T)res;
        }

        public static void SetCookie<T>(this HttpContext context, string key, T value)
        {
            context.Items[key] = value;
        }

        public static string GetCookie(this HttpContext context, string key)
        {
            return context.Request.Cookies[key];
        }
    }
}
