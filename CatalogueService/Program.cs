using CatalogueService.Entities;
using Common.MongoDB;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    //Suppress Async In Route Names
    options.SuppressAsyncSuffixInActionNames = false;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Mongo Configuration
builder.Services.AddMongo()
    .AddMongoRepo<Item>("items");

//MassTransit
builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((context, configurator) =>
    {
        var serviceName = builder.Configuration.GetSection("ServiceSettings")["Name"];

        var rabbitMqHost = builder.Configuration.GetSection("RabbitMQSettings")["Host"];
        configurator.Host(rabbitMqHost);
        configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceName,false));
    });
});


//AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
