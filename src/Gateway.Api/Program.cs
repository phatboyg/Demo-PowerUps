using Gateway.Contracts;
using MassTransit;

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

app.MapPost("/order/submit", async (IRequestClient<RequestShipmentStatus> publishEndpoint, string shipmentNumber) =>
{
    var response = await publishEndpoint.GetResponse<ShipmentStatus>(new RequestShipmentStatus { ShipmentNumber = shipmentNumber });

    return Results.Ok(value: response.Message);
});

app.Run();