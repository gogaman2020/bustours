using System;
using System.Net;
using Infrastructure.Common.Helpers;
using Infrastructure.Common.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Web.Security
{
    public static class ExceptionMiddlewareExtensions
    {
        static private readonly ILogger _logger;

        static ExceptionMiddlewareExtensions()
        {
            _logger = ApplicationLogging.CreateLogger("ExceptionMiddlewareExtensions");
        }

        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
                {
                    appError.Run(async context =>
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            context.Response.ContentType = "application/json";
            
                            var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                            if (contextFeature != null)
                            {
                                _logger.LogError($"Error: {contextFeature.Error}");
                                await context.Response.WriteAsync($"Internal Server Error. {contextFeature.Error.Message}");
                            }
                        });
                });
        }
    }
}
