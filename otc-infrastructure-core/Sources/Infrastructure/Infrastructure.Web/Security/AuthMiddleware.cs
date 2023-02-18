using System;
using System.Threading.Tasks;
using Infrastructure.Common.DI;
using Infrastructure.Security.SecurityTokens;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Web.Security
{
    [InjectAsTransient]
    public class AuthMiddleware : IMiddleware
    {
        private readonly ISecurityTokensSetter _tokensSetter;

        public AuthMiddleware(ISecurityTokensSetter tokensSetter)
        {
            _tokensSetter = tokensSetter;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Method != "OPTIONS")
            {
                _tokensSetter.SetTokens(context);
                try
                {
                    var userInfo = _tokensSetter.UserInfo;
                    if (userInfo == null)
                    {
                        throw new ApplicationException("Session is expired");
                    }
                }
                catch (Exception e)
                {
                    throw new ApplicationException(e.Message + " Querypath: " + context.Request.Path, e);
                }
            }
            await next(context);
        }
    }
}