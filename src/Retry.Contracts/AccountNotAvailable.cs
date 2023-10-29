namespace Retry.Contracts;

public record AccountNotAvailable
{
    public string? AccountNumber { get; init; }
    public string? Reason { get; init; }
}