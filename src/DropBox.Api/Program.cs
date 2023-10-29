using DropBox.Api.Models;
using DropBox.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        // using default `localhost`

        cfg.UseDelayedMessageScheduler();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapPost("/order/submit", async (IPublishEndpoint publishEndpoint, [FromBody] OrderModel order) =>
{
    await publishEndpoint.Publish(new SubmitOrder
    {
        OrderId = order.OrderId,
        Lines = order.Lines?.Select(line => new OrderLine
        {
            OrderId = order.OrderId,
            Index = line.Index,
            Sku = line.Sku
        }).ToList()
    });

    return Results.Accepted();
});

app.Run();