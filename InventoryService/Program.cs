using Common.MongoDB;
using InventoryService.Clients;
using InventoryService.Entities;
using Polly;
using Polly.Timeout;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Add Mongo Configuration
builder.Services.AddMongo()
    .AddMongoRepo<InventoryItem>("inventoryitems");


//Http client with jitter config
Random jitter = new Random();

builder.Services.AddHttpClient<CatalogClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7212");
}).AddTransientHttpErrorPolicy(builders => builders.Or<TimeoutRejectedException>().WaitAndRetryAsync(
    5, 
    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(jitter.Next(0,1000)),
    onRetry: (outcome, timespan, retryAttempt) =>
    {
        var serviceProvider = builder.Services.BuildServiceProvider();
        serviceProvider.GetService<ILogger<CatalogClient>>()?.LogWarning($"Delaying for {timespan.TotalSeconds} seconds, then making retry {retryAttempt}");
    }
))//Circuit breaker pattern
.AddTransientHttpErrorPolicy(builders => builders.Or<TimeoutRejectedException>().CircuitBreakerAsync(
    3, TimeSpan.FromTicks(15),
    onBreak: (outcome, timespan) =>
    {
        var serviceProvider = builder.Services.BuildServiceProvider();
        serviceProvider.GetService<ILogger<CatalogClient>>()?.LogWarning($"Opening circuit for {timespan.TotalSeconds} seconds...");
    },
    onReset: () =>
    {
        var serviceProvider = builder.Services.BuildServiceProvider();
        serviceProvider.GetService<ILogger<CatalogClient>>()?.LogWarning($"Closing circuit ...");
    }
    ))
    //Time out handler
    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));

//AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
