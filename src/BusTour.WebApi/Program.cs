using BusTour.Common.Config;
using BusTour.Scheduler.Clients;
using BusTour.Scheduler.Factories;
using BusTour.Scheduler.Helpers;
using BusTour.Scheduler.Jobs;
using BusTour.Scheduler.Services;
using Infrastructure.Web;
using Infrastructure.Web.Controllers;
using Infrastructure.Web.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System.Collections.Generic;

namespace BusTour.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AuthConfig.UseAuthMiddleware = false;
            JsonConfigurator.Configure = (options) =>
            {
                options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                options.SerializerSettings.DateParseHandling = DateParseHandling.DateTime;
            };

            WebAppRunner<BusTourServiceStartup>.Start(args, RegisterServices, null, Configure);
        }

        private static void RegisterServices(IServiceCollection services)
        {
            services
                .AddSingleton<CancelUnpaidOrdersJob>()
                .AddSingleton<SendNoificationsJob>()
                .AddSingleton<IJobFactory, SingletonJobFactory>()
                .AddSingleton<ISchedulerFactory, StdSchedulerFactory>()
                .AddSingleton<IBusTourClient, BusTourClient>()
                .AddHostedService<QuartzHostedService>();
        }

        private static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UsePathBase("/bustour_webapi");

            if (env.EnvironmentName != "Development")
            {
                app.UseSwagger(c =>
                {
                    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                    {
                        swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"https://{httpReq.Host.Value}{"/bustour_webapi"}" } };
                    });
                });
            }

            var loggerFactory = app.ApplicationServices.GetService<ILoggerFactory>();
            ApplicationLogging.LoggerFactory = loggerFactory;
        }
    }

    public class BusTourServiceStartup : BaseStartup
    {
        public BusTourServiceStartup(IConfiguration configuration)
            : base(configuration)
        {
        }
    }
}
