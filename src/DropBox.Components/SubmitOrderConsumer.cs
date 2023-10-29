using DropBox.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DropBox.Components;

public class SubmitOrderConsumer :
    IConsumer<SubmitOrder>
{
    readonly ILogger<SubmitOrderConsumer> _logger;

    public SubmitOrderConsumer(ILogger<SubmitOrderConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<SubmitOrder> context)
    {
        _logger.LogInformation("Submit Order Consumer: {OrderId}", context.Message.OrderId);

        return Task.CompletedTask;
    }
}