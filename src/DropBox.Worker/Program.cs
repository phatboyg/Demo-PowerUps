using DropBox.Components;
using MassTransit;

var host = Host.CreateDefaultBuilder(args)
    .UseMassTransit((host, x) =>
    {
        x.AddDelayedMessageScheduler();

        x.AddConsumer<SubmitOrderConsumer>();

        x.AddConfigureEndpointsCallback((context, _, cfg) =>
        {
            cfg.UseMessageRetry(r => r.Intervals(10, 50, 100, 1000));
            cfg.UseMessageScope(context);
            cfg.UseInMemoryOutbox(context);
        });
        
        x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("drop-box-"));

        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.UseDelayedMessageScheduler();

            cfg.ConfigureEndpoints(context);
        });
    })
    .Build();

host.Run();