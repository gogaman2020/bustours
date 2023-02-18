using Infrastructure.Common.Plugins;
using Infrastructure.Web.InfrastructurePlugin;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Infrastructure.Web
{
    public abstract class BaseStartup
    {
        protected BaseStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddRazorPages();
            services.AddControllersWithViews();
            //TODO: не видит вьюхи даже с кодом ниже.
            //.AddApplicationPart(WebAppBase.StartAssembly)
            //.AddApplicationPart(Assembly.GetExecutingAssembly())
            //.AddRazorRuntimeCompilation();

            services.AutoRegistration(Configuration);
            WebAppBase.Register?.Invoke(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();

            app.AutoConfigure();
            WebAppBase.Configure?.Invoke(app);
            WebAppBase.ConfigureWithEnv?.Invoke(app, env);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=bundles}/{action=Index}/{id?}");

                endpoints.MapFallbackToController("Get", "Version");
            });
        }
    }
}