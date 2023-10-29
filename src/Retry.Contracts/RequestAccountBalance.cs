namespace Retry.Contracts;

public record RequestAccountBalance
{
    public string? AccountNumber { get; init; }
}