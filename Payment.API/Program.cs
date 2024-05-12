using MassTransit;
using Payment.API.Consumers;
using Shared.Settings;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMassTransit(configure =>
{
    configure.AddConsumer<PaymentStartedEventConsumer>();
    configure.UsingRabbitMq((context, _configure) =>
    {
        _configure.Host(builder.Configuration["RabbitMQ"]);
        _configure.ReceiveEndpoint(RabbitMQSettings.Payment_StartedEventQueue, e => e.ConfigureConsumer<PaymentStartedEventConsumer>(context));
    });
});


var app = builder.Build();





app.Run();
