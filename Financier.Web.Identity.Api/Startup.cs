using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IdentityServer4;
using IdentityServer4.Models;

using Financier.Web.Identity.Data;
using Financier.Web.Identity.Models;

namespace Financier.Web.Identity
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
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")));
            services
                .AddDefaultIdentity<ApplicationUser>(options =>
                {
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.MaxValue;
                    options.SignIn.RequireConfirmedAccount = true;
                })
                .AddRoles<IdentityRole>() // Maybe remove?
                .AddEntityFrameworkStores<ApplicationDbContext>()
                // .AddUserManager<ApplicationUserManager>() // Maybe remove?
                // .AddDefaultTokenProviders()
                ;

            services
                .AddIdentityServer()
                .AddInMemoryIdentityResources(new[] {
                    new IdentityResources.Email()
                })
                // .AddInMemoryApiResources()
                // .AddApiAuthorization<ApplicationUser, ApplicationDbContext>()
                .AddInMemoryClients(new[] {
                    new Client
                    {
                        ClientId = "client",
                        ClientSecrets = { new Secret("secret".Sha256()) },

                        AllowedGrantTypes = GrantTypes.ClientCredentials,
                        // scopes that client has access to
                        AllowedScopes = { "api1" }
                    },
                })
                .AddAspNetIdentity<ApplicationUser>()
                .AddDeveloperSigningCredential()
            ;

            services.AddAuthorization();

            services.AddAuthentication()
                .AddJwtBearer(options => {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    // options.ClaimsIssuer = "https://localhost:5001";
                    // options.Configuration.ClaimsSupported.Add("email");
                    // options.Configuration.ClaimsSupported.Add("name");
                })
                .AddIdentityServerJwt()
            ;

            services.Configure<IdentityOptions>(options => {
            });
            services.AddControllersWithViews();
            services.AddRazorPages();
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseHttpsRedirection();
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
