using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using JWT;
using JWT.Algorithms;
using JWT.Extensions.AspNetCore;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

// TODO: place in configuration
const string PUBLIC_KEY = "ssh-rsa AAAAB3NzaC1yc2EAAAADAQABAAABgQDY4Y0WuEiY/2EdF+qZCUlgM3mfQ1jpyPYj98+HDpYN8u67QMAS2UQMoab2MeSjkGlSs98x4vFlvmrqxMuu5gWqds27bIe5wjMFrSLk9rWyIdru+UvmUdFVvOsqSj/AE/2bFJH3HNUfC7QcLv0h7LhkT5Rp/q8OgmUepBpQxkO1yMOhv7+pb11lxNUiFpjFRMIn4mKazG4mcZV49HhgGVWLmw04RReGcjR5Nhxt0xbOXUwKN5J8tZbfFk/cy74CUNALhmlnzqI9OduggjOCNyabiJ29RZ7lwPURn9ZlPEwJe95/JS2KrzjNzbmXaUbrhUjtPvyM7uZEX3BdnQNVpgJ8Ug3/M9m8XuWXxJzlUh27g6JaiYdRYFDutSXFetqCYpM0zgHWAxuh1OPXCm3D7qAjPM5HQLza5olWaVzAc1UmErm3XCbuo6BjrYr9NexvuUrvdlxFdszZI/1h54EK0UedGQSeNvaopRROaqpcj6FmogU6m/NxTLwCr0G9DPoGyIs= Financier";
var privateKey = File.ReadAllText("./financier_rsa");
var certifcationPath = "root.cert";
var certificate = new X509Certificate(certifcationPath);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//         options.UseSqlite(connectionString));
// builder.Services.AddDatabaseDeveloperPageExceptionFilter();
// builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//     .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();
// builder.Services.AddServerSideBlazor();
// builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtAuthenticationDefaults.AuthenticationScheme;
    });

builder.Services.AddSingleton<IAlgorithmFactory>(
    new RSAlgorithmFactory(() => new X509Certificate2(certificate))
);

builder.Services.AddControllers();

// builder.Services.AddSingleton

var app = builder.Build();

    // Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// TODO: Do we need these?
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "User",
    pattern: "{controller=User}/{action=Index}/{id?}"
);
app.MapFallbackToPage("/_Host");

app.Run();
