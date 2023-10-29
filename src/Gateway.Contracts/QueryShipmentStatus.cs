namespace Gateway.Contracts;

public record QueryShipmentStatus
{
    public string ShipmentNumber { get; init; } = null!;
}