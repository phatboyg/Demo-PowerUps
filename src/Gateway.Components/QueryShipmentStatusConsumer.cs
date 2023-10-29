using Gateway.Contracts;
using MassTransit;

namespace Gateway.Components;

public class QueryShipmentStatusConsumer :
    IConsumer<QueryShipmentStatus>
{
    public async Task Consume(ConsumeContext<QueryShipmentStatus> context)
    {
        await Task.Delay(Random.Shared.Next(1000, 3000), context.CancellationToken);

        await context.RespondAsync(new ShipmentStatus
        {
            ShipmentNumber = context.Message.ShipmentNumber,
            LastUpdated = DateTimeOffset.Now,
            Status = Random.Shared.Next(1, 3) switch
            {
                1 => "Waiting for pickup",
                2 => "In Transit",
                3 => "Delivered",
                _ => "Unknown"
            }
        });
    }
}