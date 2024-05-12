using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Consumers;
using Order.API.Contexts;
using Order.API.ViewModels;
using Shared.OrderEvents;
using Shared.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddMassTransit(configure =>
{
    configure.AddConsumer<OrderCompletedEventConsumer>();
    configure.AddConsumer<OrderFailedEventConsumer>();
    configure.UsingRabbitMq((context, _configure) =>
    {
        _configure.Host(builder.Configuration["RabbitMQ"]);
        _configure.ReceiveEndpoint(RabbitMQSettings.Order_OrderCompletedEventQueue, e => e.ConfigureConsumer<OrderCompletedEventConsumer>(context));
        _configure.ReceiveEndpoint(RabbitMQSettings.Order_OrderFailedEventQueue, e => e.ConfigureConsumer<OrderFailedEventConsumer>(context));
    });
});


builder.Services.AddDbContext<OrderAPIDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
});

var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();



app.MapPost("/create-order", async (CreateOrderVM model,OrderAPIDbContext context,ISendEndpointProvider sendEndpointProvider) =>
{
    Order.API.Entities.Order order = new()
    {
        BuyerId = Guid.Parse(model.BuyerId),
        OrderItems = model.OrderItems.Select(oi => new Order.API.Entities.OrderItem
        {
            ProductId = Guid.Parse(oi.ProductId),
            Count = oi.Count,
            Price = oi.Price
        }).ToList(),
        Id = Guid.NewGuid(),
        OrderStatu = Order.API.Enums.OrderStatus.Suspended,
        TotalPrice = model.OrderItems.Sum(oi => oi.Price * oi.Count),          
    };

    await context.Orders.AddAsync(order);
    await context.SaveChangesAsync();

    OrderStartedEvent orderStartedEvent = new()
    {
        BuyerId = order.BuyerId.ToString(),
        OrderId = order.Id.ToString(),
        TotalPrice = order.TotalPrice,
        OrderItems = order.OrderItems.Select(oi => new Shared.Messages.OrderItemMessage
        {
            Count = oi.Count,
            ProductId = oi.ProductId,
            Price = oi.Price,
        }).ToList()
    };

    var sendEndPoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.StateMachineQueue}"));
    await sendEndPoint.Send(orderStartedEvent);
    
});


app.Run();
