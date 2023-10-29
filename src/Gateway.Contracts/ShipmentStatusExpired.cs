namespace Gateway.Contracts;

public record ShipmentStatusExpired
{
    public Guid CorrelationId { get; init; }
}