using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Models.DataManager;
using Newtonsoft.Json;
using WebApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<McbaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MvcMovieContext")));

builder.Services.AddScoped<CustomerManager>();
builder.Services.AddScoped<LoginManager>();
builder.Services.AddScoped<AccountManager>();
builder.Services.AddScoped<TransactionManager>();
builder.Services.AddScoped<BillPayManager>();
//builder.Services.AddTransient<MovieManager>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed data.
using(var scope = app.Services.CreateScope())
{
    using var client = new HttpClient();
    var result = client.GetStringAsync("https://coreteaching01.csit.rmit.edu.au/~e103884/wdt/services/customers/").Result;


    // converts JSON into customer objects
    var customers = JsonConvert.DeserializeObject<List<Customer>>(result, new JsonSerializerSettings
    {
        DateFormatString = "dd/MM/yyyy hh:mm:ss tt"
    });
    var services = scope.ServiceProvider;
    try
    {
        SeedData.Initialize(services, customers);
    }
    catch(Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

// .NET 6 Minimal APIs - Simple Example.
// See here for more information:
// https://docs.microsoft.com/en-au/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-6.0

// GET api/UsingMapGet?name=Matthew&repeat=100
app.MapGet("api/UsingMapGet", (string name, int? repeat) =>
{
    if(string.IsNullOrWhiteSpace(name))
        name = "(empty)";
    if(repeat is null or < 1)
        repeat = 1;

    return string.Join(' ', Enumerable.Repeat(name, repeat.Value));
});

app.Run();
