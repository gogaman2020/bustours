using Infrastructure.Security.SecurityTokens;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Web.Security
{
    public static class SecurityTokensSetterExtensions
    {
        public static void SetTokens(this ISecurityTokensSetter setter, HttpContext httpContext)
        {
            setter.SetTokens(httpContext.GetCookie(setter.SecurityTokenName),
                httpContext.GetCookie(setter.SecurityTokenV2Name));
        }
    }
}