using Kentico.Activities.Web.Mvc;
using Kentico.CampaignLogging.Web.Mvc;
using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.Newsletters.Web.Mvc;
using Kentico.OnlineMarketing.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Scheduler.Web.Mvc;
using Kentico.Web.Mvc;
using MEDIOClinic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BlankSiteCore
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }


        public Startup(IWebHostEnvironment environment)
        {
            Environment = environment;
        }

        private const string CONSTRAINT_FOR_NON_ROUTER_PAGE_CONTROLLERS = "Search|controller";

        public const string DEFAULT_WITHOUT_LANGUAGE_PREFIX_ROUTE_NAME = "DefaultWithoutLanguagePrefix";

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Enable desired Kentico Xperience features
            var kenticoServiceCollection = services.AddKentico(features =>
            {
                features.UsePageBuilder();
                features.UseActivityTracking();
                features.UseABTesting();
                features.UseWebAnalytics();
                features.UseEmailTracking();
                features.UseCampaignLogger();
                features.UseScheduler();
                features.UsePageRouting(new PageRoutingOptions { EnableAlternativeUrls = true, CultureCodeRouteValuesKey = "culture" });
            });

            if (Environment.IsDevelopment())
            {
                // By default, Xperience sends cookies using SameSite=Lax. If the administration and live site applications
                // are hosted on separate domains, this ensures cookies are set with SameSite=None and Secure. The configuration
                // only applies when communicating with the Xperience administration via preview links. Both applications also need 
                // to use a secure connection (HTTPS) to ensure cookies are not rejected by the client.
                kenticoServiceCollection.SetAdminCookiesSameSiteNone();

                // By default, Xperience requires a secure connection (HTTPS) if administration and live site applications
                // are hosted on separate domains. This configuration simplifies the initial setup of the development
                // or evaluation environment without a the need for secure connection. The system ignores authentication
                // cookies and this information is taken from the URL.
                kenticoServiceCollection.DisableVirtualContextSecurityForLocalhost();
            }

            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            services.AddAuthentication();
            // services.AddAuthorization();

            services.AddLocalization()
                    .AddControllersWithViews()
                    .AddViewLocalization()
                    .AddDataAnnotationsLocalization(options =>
                    {
                        options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResources));
                    });

            services.Configure<KenticoRequestLocalizationOptions>(options =>
            {
                options.RequestCultureProviders.Add(new RouteDataRequestCultureProvider
                {
                    RouteDataStringKey = "culture",
                    UIRouteDataStringKey = "culture"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePagesWithReExecute("/error/{0}");

            app.UseStaticFiles();

            app.UseKentico();

            app.UseCookiePolicy();
            app.UseCors();

            app.UseAuthentication();
            // app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.Kentico().MapRoutes();

                endpoints.MapControllerRoute(
                   name: "error",
                   pattern: "error/{code}",
                   defaults: new { controller = "HttpErrors", action = "Error" }
                );

                endpoints.MapControllerRoute(
                   name: "default",
                   pattern: $"{{culture}}/{{controller}}/{{action}}",
                   constraints: new
                   {
                       culture = new SiteCultureConstraint { HideLanguagePrefixForDefaultCulture = false },
                       controller = CONSTRAINT_FOR_NON_ROUTER_PAGE_CONTROLLERS
                   }
                );


                endpoints.MapControllerRoute(
                    name: DEFAULT_WITHOUT_LANGUAGE_PREFIX_ROUTE_NAME,
                    pattern: "{controller}/{action}",
                    constraints: new
                    {
                        controller = CONSTRAINT_FOR_NON_ROUTER_PAGE_CONTROLLERS
                    }
                );


            });
        }
    }
}
