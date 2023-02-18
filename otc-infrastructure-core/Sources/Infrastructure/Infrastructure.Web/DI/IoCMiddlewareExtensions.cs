using System.Linq;
using Infrastructure.Common.DI;
using Microsoft.AspNetCore.Builder;

namespace Infrastructure.Web.DI
{
    public static class IoCMiddlewareExtensions
    {
        public static IApplicationBuilder UseIoCMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<IoCMiddleware>();
        }
    }
}