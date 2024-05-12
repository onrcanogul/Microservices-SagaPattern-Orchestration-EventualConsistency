using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Settings;
using Stock.API.Consumers;
using Stock.API.Contexts;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMassTransit(configure =>
{
    configure.AddConsumer<OrderCreatedEventConsumer>();
    configure.AddConsumer<StockRollbackMessageConsumer>();
    configure.UsingRabbitMq((context, _configure) =>
    {
        _configure.Host(builder.Configuration["RabbitMQ"]);
        _configure.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedEventQueue, e => e.ConfigureConsumer<OrderCreatedEventConsumer>(context));
        _configure.ReceiveEndpoint(RabbitMQSettings.Stock_RollbackMessageQueue, e => e.ConfigureConsumer<StockRollbackMessageConsumer>(context));
    });
});

builder.Services.AddDbContext<StockAPIDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
});

var app = builder.Build();


using var scope = builder.Services.BuildServiceProvider();
StockAPIDbContext context = scope.GetService<StockAPIDbContext>();


if(!await context.Stocks.AnyAsync())
{
    await context.Stocks.AddAsync(new Stock.API.Entities.Stock() { ProductId = Guid.NewGuid(), Count = 10 });
    await context.Stocks.AddAsync(new Stock.API.Entities.Stock() { ProductId = Guid.NewGuid(), Count = 10 });
    await context.SaveChangesAsync();
}



app.Run();
