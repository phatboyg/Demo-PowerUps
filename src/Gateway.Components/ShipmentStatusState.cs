using MassTransit;

namespace Gateway.Components;

public class ShipmentStatusState :
    SagaStateMachineInstance
{
    public string? CurrentState { get; set; }
    public string? LastStatus { get; set; }
    public DateTimeOffset? LastUpdated { get; set; }

    public string ShipmentNumber { get; set; } = null!;
    public Guid? ExpirationToken { get; set; }

    public Guid CorrelationId { get; set; }
}