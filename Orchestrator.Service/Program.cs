using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Orchestrator.Service.StateDbContext;
using Orchestrator.Service.StateInstances;
using Orchestrator.Service.StateMachines;
using Shared.Settings;


var builder = Host.CreateApplicationBuilder(args);



builder.Services.AddMassTransit(configure =>
{
    configure.AddSagaStateMachine<OrderStateMachine,OrderStateInstance>().EntityFrameworkRepository(opt =>
    {
        opt.LockStatementProvider = new PostgresLockStatementProvider();

        opt.AddDbContext<DbContext, OrderStateDbContext>((provider, _builder) =>
        {
            _builder.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
        });
    });
    configure.UsingRabbitMq((context, _configure) =>
    {
        _configure.Host(builder.Configuration["RabbitMQ"]);

        _configure.ReceiveEndpoint(RabbitMQSettings.StateMachineQueue, e => e.ConfigureSaga<OrderStateInstance>(context));
    });
});

var host = builder.Build();
host.Run();
