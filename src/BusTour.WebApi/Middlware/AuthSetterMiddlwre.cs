using BusTour.Common.Services;
using Infrastructure.Common.DI;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BusTour.WebApi.Middlware
{
    [InjectAsTransient]
    public class AuthSetterMiddlwre : IMiddleware
    {
        private readonly IUserAuthSetter _userAuthSetter;

        public AuthSetterMiddlwre(IUserAuthSetter userAuthSetter)
        {
            _userAuthSetter = userAuthSetter;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _userAuthSetter.Set(context.User);
            await next(context);
        }
    }
}
