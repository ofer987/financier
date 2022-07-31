using GraphQL;
using GraphQL.Instrumentation;
using GraphQL.Server;
using GraphQL.Types;
using GraphQL.MicrosoftDI;
using GraphQL.SystemTextJson;

using Financier.Common;
using Financier.Web.GraphQL;
using Financier.Web.GraphQL.CashFlows;
using Financier.Web.GraphQL.Items;

const string DevelopmentPolicy = "CORS_POLICY_LOCALHOST";

var builder = WebApplication.CreateBuilder(args);

// GraphQL
builder.Services.AddSingleton<IFieldMiddleware, UserContextMiddleware>();
builder.Services.AddSingleton<ISchema, CashFlowSchema>();
builder.Services.AddSingleton<ISchema, ItemSchema>();
builder.Services.AddGraphQL(builder => builder
    .AddHttpMiddleware<CashFlowSchema>()
    .AddHttpMiddleware<ItemSchema>()
    .AddUserContextBuilder(c => new UserContext(c.Request.Headers.Authorization.First()))
    .AddSystemTextJson()
    .AddErrorInfoProvider(options => {
        options.ExposeExtensions = true;
        options.ExposeExceptionStackTrace = true;
    })
    .AddSchema<CashFlowSchema>()
    .AddSchema<ItemSchema>()
    .AddGraphTypes(typeof(CashFlowSchema).Assembly)
);

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
    routes.MapGraphQL<ItemSchema>("/graphql/items");
});

app.UseGraphQL<CashFlowSchema>();
app.UseGraphQL<ItemSchema>();

app.Run();
