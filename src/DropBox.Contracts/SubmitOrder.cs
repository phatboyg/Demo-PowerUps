namespace DropBox.Contracts;

public record SubmitOrder
{
    public Guid OrderId { get; init; }

    public IReadOnlyList<OrderLine>? Lines { get; init; }
}