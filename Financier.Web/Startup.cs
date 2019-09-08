using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using AspNetCore.RouteAnalyzer;

using Financier.Common;

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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));
            services.AddScoped<ItemSchema>();

            // Add GraphQL
            services.AddGraphQL(options =>
                    {
                        options.EnableMetrics = true;
                        options.ExposeExceptions = true;
                    })
            .AddGraphTypes(ServiceLifetime.Scoped)
            .AddDataLoader();

            services
                .AddMvc();
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

            app.UseGraphQL<ItemSchema>();

            // app.UseWebSockets();
            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions());

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "monthlyexpenses_index",
                    template: "MonthlyExpenses/Index/year/{year}/month/{month}",
                    defaults: new { controller = "MonthlyExpenses", action = "Index" }
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
