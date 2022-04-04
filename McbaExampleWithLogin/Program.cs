using McbaExampleWithLogin.BackgroundServices;
using McbaExampleWithLogin.Data;
using McbaExampleWithLogin.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<McbaContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(McbaContext)));

    // Enable lazy loading.
    options.UseLazyLoadingProxies();
});
// Store session into Web-Server memory.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    // Make the session cookie essential.
    options.Cookie.IsEssential = true;
});

builder.Services.AddHostedService<CustomerBackgroundService>();

// Bonus Material: Store session into SQL Server.
// Please see session-commands.md file.
// Package required: Microsoft.Extensions.Caching.SqlServer
//builder.Services.AddDistributedSqlServerCache(options =>
//{
//    options.ConnectionString = builder.Configuration.GetConnectionString(nameof(McbaContext));
//    options.SchemaName = "dotnet";
//    options.TableName = "SessionCache";
//});
//builder.Services.AddSession(options =>
//{
//    // Make the session cookie essential.
//    options.Cookie.IsEssential = true;
//    options.IdleTimeout = TimeSpan.FromDays(7);
//});

builder.Services.AddControllersWithViews();

// Bonus Material: Implement global authorisation check. Also see the AuthorizeCustomerAttribute.cs file.
//builder.Services.AddControllersWithViews(options => options.Filters.Add(new AuthorizeCustomerAttribute()));

builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// Seed data.
using(var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    using var client = new HttpClient();
    var result = client.GetStringAsync("https://coreteaching01.csit.rmit.edu.au/~e103884/wdt/services/customers/").Result;


    // converts JSON into customer objects
    var customers = JsonConvert.DeserializeObject<List<Customer>>(result, new JsonSerializerSettings
    {
        DateFormatString = "dd/MM/yyyy hh:mm:ss tt"
    });
    try
    {
        // contacts the webservice
        //using var client = new HttpClient();
        //var result = client.GetStringAsync("https://coreteaching01.csit.rmit.edu.au/~e103884/wdt/services/customers/").Result;


        //// converts JSON into customer objects
        //var customers = JsonConvert.DeserializeObject<List<Customer>>(result, new JsonSerializerSettings
        //{
        //    DateFormatString = "dd/MM/yyyy hh:mm:ss tt"
        //});
        SeedData.Initialize(services, customers);
    }
    catch(Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

// Configure the HTTP request pipeline.
if(!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Home/Error");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();

app.MapDefaultControllerRoute();

app.Run();
