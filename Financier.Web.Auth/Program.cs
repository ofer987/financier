using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

using GraphQL;
using GraphQL.Server;
using GraphQL.Types;
using Financier.Web.Auth.GraphQL.CashFlows;

using Financier.Web.Auth;
using Financier.Web.Auth.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

builder.Services.AddSingleton<CashFlowSchema>();
GraphQL.MicrosoftDI.GraphQLBuilderExtensions.AddGraphQL(builder.Services)
    .AddServer(true, options => options.EnableMetrics = true)
    .AddUserContextBuilder(httpContext => new GraphQLUserContext { User = httpContext.User })
    .AddSystemTextJson()
    .AddErrorInfoProvider(options => {
        options.ExposeExtensions = true;
        options.ExposeExceptionStackTrace = true;
    })
    .AddSchema<CashFlowSchema>()
    .AddGraphTypes(typeof(CashFlowSchema).Assembly);

builder.Services.AddSingleton<CashFlowSchema>();
builder.Services.AddLogging(builder => builder.AddConsole());
builder.Services.AddHttpContextAccessor();

    // .AddGraphQL((options, provider) =>
    //         {
    //         options.EnableMetrics = true;
    //
    //         var logger = provider.GetRequiredService<ILogger<Program>>();
    //         options.UnhandledExceptionDelegate = (context) => logger.LogError($"Error occurred: {context.OriginalException.Message}");
    //         // TODO: should depend whether is dev environment
    //         // options.ExposeExceptions = true;
    //         })
    // .AddSystemTextJson()
    //     // .AddUserContextBuilder(httpContext => httpContext)
    //     .AddDataLoader();
    //     ;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseEndpoints(routes => {
    routes.MapGraphQLPlayground();

    routes.MapGraphQL<CashFlowSchema>("/graphql/cash-flows");
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
