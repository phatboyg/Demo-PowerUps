namespace Retry.Contracts;

public record AccountBalance
{
    public string? AccountNumber { get; init; }
    public decimal Balance { get; init; }
}