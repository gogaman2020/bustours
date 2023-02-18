using System;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Web.Security
{
    public static class AuthConfig
    {
        static AuthConfig()
        {
            UseAuthMiddleware = true;
        }
        public static bool UseAuthMiddleware { get; set; }
        public static Func<HttpContext, bool> Filter { get; set; }
    }
}