using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GraphQL.Server;
using AspNetCore.RouteAnalyzer;

using Financier.Common;
using Financier.Web.GraphQL.Liabilities;
using Financier.Web.GraphQL.Tags;
using Financier.Web.GraphQL.CashFlows;
using Financier.Web.GraphQL.ItemQueries;
using Financier.Web.GraphQL.Items;
using Financier.Web.GraphQL.Activities;
using Financier.Web.Data;

namespace Financier.Web
{
    public class Startup
    {
        public static string DevelopmentPolicy = "CORS_POLICY_LOCALHOST";

        public Startup(IConfiguration configuration)
        {
            Context.Environment = Financier.Common.Environments.Dev;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(options =>
            {
                // This needs to be enabled because GraphQL 2.4's
                // JSON serialiser is synchronous
                // TODO: set to false after upgrading to GraphQL 3
                options.AllowSynchronousIO = false;
            });
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));
            services.AddSingleton<CashFlowSchema>();
            services.AddSingleton<ItemSchema>();
            services.AddSingleton<TagSchema>();
            services.AddSingleton<ItemQuerySchema>();
            services.AddSingleton<FixedRateMortgageSchema>();
            services.AddSingleton<OneHomeSchema>();
            services.AddSingleton<IdentityDataContext>();
            services.AddAuthenticationCore();
            services.AddAuthorizationCore();

            // Add GraphQL
            services
                .AddGraphQL((options, provider) =>
                    {
                        options.EnableMetrics = true;

                        var logger = provider.GetRequiredService<ILogger<Startup>>();
                        options.UnhandledExceptionDelegate = (context) => logger.LogError($"Error occurred: {context.OriginalException.Message}");
                        // TODO: should depend whether is dev environment
                        // options.ExposeExceptions = true;
                    })
                .AddGraphTypes(typeof(ItemSchema))
                .AddSystemTextJson()
                // .AddUserContextBuilder(httpContext => httpContext)
                .AddDataLoader();
            ;

            // TODO: use the latest MVC routing
            // Please see
            // https://docs.microsoft.com/en-us/aspnet/core/migration/22-to-30
            // https://docs.microsoft.com/en-us/aspnet/core/migration/30-to-31
            services
                .AddMvc(options => options.EnableEndpointRouting = false);
            // .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //
            services
                .AddRouteAnalyzer();

            services.AddCors(options =>
            {
                options.AddPolicy(
                    DevelopmentPolicy,
                    builder => builder
                        .WithOrigins("http://localhost:8001")
                        .AllowAnyHeader()
                        .Build()
                );
            });

            services.AddLogging();
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<IdentityDataContext>();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(DevelopmentPolicy);
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                // app.UseHttpsRedirection();
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseGraphQLPlayground();

            app.UseRouting();
            app.UseEndpoints(routes =>
            {
                routes.MapGraphQLPlayground();

                routes.MapGraphQL<TagSchema>("/graphql/tags");
                routes.MapGraphQL<ItemSchema>("/graphql/items");
                routes.MapGraphQL<CashFlowSchema>("/graphql/cash-flows");
                routes.MapGraphQL<ItemQuerySchema>("/graphql/item-queries");
                routes.MapGraphQL<OneHomeSchema>("/graphql/one-home");
            });

            app.UseAuthentication();
            app.UseAuthorization();

            // app.MapRazorPages();

            // Please note that some of these routes are deprecated
            // TODO: Remove the deprecated routes
            app.UseMvc(routes =>
            {
                routes.MapRouteAnalyzer("/routes");
            });
        }
    }
}
