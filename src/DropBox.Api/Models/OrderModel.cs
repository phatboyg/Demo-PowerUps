namespace DropBox.Api.Models;

public record OrderModel(Guid OrderId, List<OrderLineModel>? Lines);