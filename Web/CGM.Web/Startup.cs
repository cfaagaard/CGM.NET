using CGM.Communication.Common;
using CGM.Communication.Common.Serialize;
using CGM.Communication.Extensions;
using CGM.Communication.Interfaces;
using CGM.Communication.Log;
using CGM.Data.Nightscout;
using CGM.Data.Nightscout.RestApi;
using CGM.Web.Hubs;
using CGM.Web.Hubs.HubLog;
using CommonServiceLocator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Unity;
using Unity.ServiceLocation;

namespace CGM.Web
{
    public class Startup
    {
        public Startup(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public Microsoft.Extensions.Configuration.IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //services.AddMvc(o => o.InputFormatters.Insert(0, new RawRequestBodyFormatter()));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //services.AddSignalR();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            //app.UseSignalR(routes =>
            //{
            //    routes.MapHub<DataLoggerHub>("/dataloggerhub");
            //});



            

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }

        public void ConfigureContainer(IUnityContainer container)
        {


            container.RegisterType<IStateRepository, CGM.Data.SessionStateRepository>();
            CgmSessionBehaviors behaviors = new CgmSessionBehaviors();
            behaviors.SessionBehaviors.Add(container.Resolve<NightScoutUploader>());
            container.RegisterInstance(typeof(ISessionBehaviors), behaviors);


            UnityServiceLocator locator = new UnityServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);


            //var ctx = container.Resolve<DataLoggerHub>();
            //ApplicationLogging.LoggerFactory.AddHubLogger(ctx);

            //init
            //SerializerSession session = new SerializerSession();
            //session.MinimedConfiguration();
            //session.NightscoutConfiguration();
            //session.NotifiyConfiguration();

        }
    }
}
