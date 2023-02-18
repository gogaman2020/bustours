using System.Threading.Tasks;
using Infrastructure.Common.DI;
using Infrastructure.Common.Helpers;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Web.DI
{
    [InjectAsSingleton]
    public class IoCMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            using (var iocscope = IoC.InitScopeProvider(context.RequestServices))
            {
                await next(context);
            }
        }
    }
}