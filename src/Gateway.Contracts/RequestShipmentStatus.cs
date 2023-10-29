namespace Gateway.Contracts;

public record RequestShipmentStatus
{
    public string ShipmentNumber { get; init; } = null!;
}