using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using AspNetCore.RouteAnalyzer;

using Financier.Common;
using Financier.Web.GraphQL.Liabilities;
using Financier.Web.GraphQL.Tags;
using Financier.Web.GraphQL.CashFlows;
using Financier.Web.GraphQL.ItemQueries;
using Financier.Web.GraphQL.Items;

namespace Financier.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Context.Environment = Environments.Dev;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(options =>
            {
                // NOTE this is temporary as part of the transition from 
                // .NET Core 2.2 to 3.1
                // TODO switch to asynchronous IO!!!
                options.AllowSynchronousIO = true;
            });
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));
            services.AddScoped<CashFlowSchema>();
            services.AddScoped<ItemSchema>();
            services.AddScoped<TagSchema>();
            services.AddScoped<ItemQuerySchema>();
            services.AddScoped<FixedRateMortgageSchema>();

            // Add GraphQL
            services
                .AddGraphQL(options =>
                    {
                        options.EnableMetrics = true;
                        // TODO: should depend whether is dev environment
                        options.ExposeExceptions = true;
                    })
                .AddGraphTypes(ServiceLifetime.Scoped)
                .AddDataLoader();

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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseGraphQL<TagSchema>("/graphql/tags");
            app.UseGraphQL<ItemSchema>("/graphql/items");
            app.UseGraphQL<CashFlowSchema>("/graphql/cash-flows");
            app.UseGraphQL<ItemQuerySchema>("/graphql/item-queries");
            app.UseGraphQL<FixedRateMortgageSchema>("/graphql/fixed-rate-mortgage");

            // app.UseWebSockets();
            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions());

            app.UseMvc(routes =>
            {
                routes.MapAreaRoute(
                    name: "Expenses/MyHome#GetMonthlyPayments",
                    areaName: "Expenses",
                    template: "expenses/my-home/get-monthly-payments",
                    defaults: new { controller = "MyHome", action = "GetMonthlyPayments" }
                );

                routes.MapRoute(
                    name: "Items#Chart",
                    template: "Items/chart",
                    defaults: new { controller = "Charts", action = "Chart" }
                );

                routes.MapRoute(
                    name: "CashFlows#GetYear",
                    template: "CashFlows/year/{year}",
                    defaults: new { controller = "CashFlows", action = "GetYear" }
                );

                routes.MapRoute(
                    name: "CashFlows#GetMonth",
                    template: "CashFlows/year/{year}/month/{month}",
                    defaults: new { controller = "CashFlows", action = "GetMonth" }
                );

                routes.MapRoute(
                    name: "AllItems#GetMonth",
                    template: "AllItems/Get/year/{year}/month/{month}",
                    defaults: new { controller = "AllItems", action = "GetMonth" }
                );

                routes.MapRoute(
                    name: "AllItems#GetYear",
                    template: "AllItems/Get/year/{year}",
                    defaults: new { controller = "AllItems", action = "GetYear" }
                );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "MonthlyStatements.Get",
                    template: "MonthlyStatements/Get/year/{year}",
                    defaults: new { controller = "MonthlyStatements", action = "Get" }
                );

                routes.MapRouteAnalyzer("/routes");
            });
        }
    }
}
