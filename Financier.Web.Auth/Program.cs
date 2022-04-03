using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using GraphQL;
using GraphQL.Server;
using Financier.Web.Auth.GraphQL.CashFlows;

using Financier.Web.Auth.GraphQL;
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
    .AddUserContextBuilder(context => new UserContext { User = context.User })
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

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(routes => {
    routes.MapGraphQLPlayground();

    routes.MapGraphQL<CashFlowSchema>("/graphql/cash-flows");
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapRazorPages();
app.UseGraphQL<CashFlowSchema>();

app.Run();
