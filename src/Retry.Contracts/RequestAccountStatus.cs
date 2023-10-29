namespace Retry.Contracts;

public record RequestAccountStatus
{
    public string? AccountNumber { get; init; }
}