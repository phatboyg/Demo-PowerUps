using Gateway.Components;
using MassTransit;
using MassTransit.Components;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => { })
    .UseMassTransit((host, x) =>
    {
        x.AddDelayedMessageScheduler();

        x.AddConsumer<QueryShipmentStatusConsumer>();
        x.AddSagaStateMachine<ShipmentStatusStateMachine,ShipmentStatusState>()
            .InMemoryRepository();
        
        x.AddSagaStateMachine<RequestStateMachine, RequestState>()
            .InMemoryRepository();

        x.AddConfigureEndpointsCallback((context, _, cfg) =>
        {
            cfg.UseMessageRetry(r => r.Intervals(10, 50, 100, 1000));
            cfg.UseMessageScope(context);
            cfg.UseInMemoryOutbox(context);
        });

        x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("gateway-"));

        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.UseDelayedMessageScheduler();

            cfg.ConfigureEndpoints(context);
        });
    })
    .Build();

host.Run();