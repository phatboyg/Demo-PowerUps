namespace Gateway.Contracts;

public record ShipmentStatus
{
    public string ShipmentNumber { get; init; } = null!;
    public DateTimeOffset? LastUpdated { get; init; }

    public string? Status { get; init; }
}