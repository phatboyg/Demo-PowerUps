namespace DropBox.Contracts;

public record OrderLine
{
    public Guid OrderId { get; init; }
    public int Index { get; init; }
    public string? Sku { get; init; }
}