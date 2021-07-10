using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Financier.Web.Auth.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;

namespace Financier.Web.Auth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
                // options.UseOpenIddict();
            });

            services
                .AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddRazorPages();

            // services
            //     .Configure<IdentityOptions>(options =>
            //     {
            //         options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
            //         options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
            //         options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
            //     });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Audience = "http://localhost:5000";
                    options.Authority = "http://localhost:5000/identity";

                    // TODO: Set to false only for Development and Test environments
                    options.RequireHttpsMetadata = false;
                });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            // services.AddOpenIddict()
            //     .AddCore(options =>
            //     {
            //         options
            //             .UseEntityFrameworkCore()
            //             .UseDbContext<ApplicationDbContext>();
            //     })
            //     .AddServer(options =>
            //     {
            //         options.UseMvc();
            //
            //         options
            //             .EnableAuthorizationEndpoint("/connect/authorize")
            //             .EnableLogoutEndpoint("/connect/logout")
            //             .EnableTokenEndpoint("/connect/token")
            //             .EnableUserinfoEndpoint("/connect/userinfo");
            //
            //         options
            //             .AllowAuthorizationCodeFlow()
            //             .AllowPasswordFlow()
            //             .AllowRefreshTokenFlow();
            //
            //         options
            //             .RegisterScopes(
            //                 OpenIddictConstants.Scopes.Email,
            //                 OpenIddictConstants.Scopes.Profile,
            //                 OpenIddictConstants.Scopes.Roles
            //             );
            //
            //         options
            //             .EnableRequestCaching();
            //
            //         options
            //             .DisableHttpsRequirement();
            //     })
            //     .AddValidation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();

                endpoints
                    .MapGet("/Expenses", async context =>
                        await context.Response.WriteAsync("Hello Expenses")
                    );

                endpoints
                    .MapGet("/Expenses/{arg}", async context =>
                        await context.Response.WriteAsync("Hello Expenses")
                    );

                endpoints
                    .MapControllerRoute(
                        name: "Expenses",
                        pattern: "{controller=Expenses}/{action=Index}/{innerRoute}"
                    )
                    .RequireAuthorization();

            });

            // app.UseMvc(routes =>
            // {
            //     routes.MapRoute(
            //         name: "Expenses",
            //         template: "Expenses/{inner_route}",
            //         defaults: new { controller = "Expenses", action = "GetRoute" }
            //     );
            // });
            //
            // app.ApplicationServices
        }
    }
}
