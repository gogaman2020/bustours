using System;
using System.Linq;
using System.Reflection;
using Infrastructure.Common.Plugins;
using Infrastructure.Web.InfrastructurePlugin;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Infrastructure.Web.Controllers
{
    [WebPluginOrder(1000)]
    public class ControllersWebPlugin : IInfrastructureWebPlugin
    {
        public void RegisterServices(IServiceCollection services, Type[] types, IConfiguration configuration)
        {
            // var controllerAssemblies = Types.AvailableTypes
            //     .Where(t => t.IsClass && typeof(ControllerBase).IsAssignableFrom(t))
            //     .Select(t => t.Assembly)
            //     .Distinct();
            //
            // var installer = services.AddMvc();
            // foreach (var ass in controllerAssemblies)
            // {
            //     installer.AddApplicationPart(ass);
            // }
            // installer

            services.AddMvc()
                .AddApplicationPart(WebAppBase.StartAssembly)
                .AddApplicationPart(Assembly.GetExecutingAssembly())
                .AddControllersAsServices()
                // .AddJsonOptions(o =>
                // {
                //     o.JsonSerializerOptions.DictionaryKeyPolicy = null;
                //     o.JsonSerializerOptions.PropertyNamingPolicy = null;
                // })
                .AddNewtonsoftJson(options =>
                {
                    JsonConfigurator.Configure?.Invoke(options);
                });
            services.Replace(ServiceDescriptor.Singleton<IControllerActivator, ServiceBasedControllerActivator>());
        }

        public void Configure(IApplicationBuilder app, IConfiguration configuration)
        {
        }
    }
}