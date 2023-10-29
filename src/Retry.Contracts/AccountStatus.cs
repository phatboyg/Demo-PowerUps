namespace Retry.Contracts;

public record AccountStatus
{
    public string? AccountNumber { get; init; }
    public string? Status { get; init; }
}