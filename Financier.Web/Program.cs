using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using GraphQL;
using GraphQL.DI;
using GraphQL.Server.Authorization.AspNetCore;
using GraphQL.Server.Transports.AspNetCore;
using GraphQL.Server;
using GraphQL.Types;
using GraphQL.MicrosoftDI;
using GraphQL.SystemTextJson;

using Financier.Common;
using Financier.Web.Data;
using Financier.Web.Models;
using Financier.Web.GraphQL;
using Financier.Web.GraphQL.CashFlows;

const string DevelopmentPolicy = "CORS_POLICY_LOCALHOST";

var builder = WebApplication.CreateBuilder(args);

// GraphQL
builder.Services.AddSingleton<CashFlowSchema>();
builder.Services.AddGraphQL(builder => builder
    .AddHttpMiddleware<CashFlowSchema>()
    .AddUserContextBuilder(context => {
        return new UserContext(context.Request.Headers.Authorization.First());
    })
    .AddSystemTextJson()
    .AddErrorInfoProvider(options => {
        options.ExposeExtensions = true;
        options.ExposeExceptionStackTrace = true;
    })
    .AddSchema<CashFlowSchema>()
    .AddGraphTypes(typeof(CashFlowSchema).Assembly)
);
builder.Services.AddSingleton<CashFlowSchema>();

// CORS
builder.Services.AddCors(options => {
    options.AddPolicy(
        DevelopmentPolicy,
        builder => builder
            .WithOrigins("https://localhost:7168")
            .AllowAnyHeader()
            .Build()
    );
});

// Expenses Database
Context.Environment = Financier.Common.Environments.Dev;
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseCors(DevelopmentPolicy);

    app.UseMigrationsEndPoint();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseEndpoints(routes => {
    routes.MapGraphQLPlayground();

    routes.MapGraphQL<CashFlowSchema>("/graphql/cash-flows");
});

app.UseGraphQL<CashFlowSchema>();

app.Run();
