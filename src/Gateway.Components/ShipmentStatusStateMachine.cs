using Gateway.Contracts;
using MassTransit;

namespace Gateway.Components;

public class ShipmentStatusStateMachine :
    MassTransitStateMachine<ShipmentStatusState>
{
    public ShipmentStatusStateMachine()
    {
        Request(() => QueryRequest, x => x.Timeout = TimeSpan.FromSeconds(30));
        
        InstanceState(x => x.CurrentState);

        Event(() => ShipmentStatusRequested, x =>
        {
            x.CorrelateBy(s => s.ShipmentNumber, context => context.Message.ShipmentNumber)
                .SelectId(context => context.RequestId ?? NewId.NextGuid());

            x.InsertOnInitial = true;

            x.SetSagaFactory(context => new ShipmentStatusState
            {
                CorrelationId = context.CorrelationId ?? NewId.NextGuid(),
                ShipmentNumber = context.Message.ShipmentNumber
            });
        });

        Schedule(() => ShipmentStatusExpired, x => x.ExpirationToken);

        During(Initial, StatusExpired,
            When(ShipmentStatusRequested)
                .Request(QueryRequest, context => new QueryShipmentStatus()
                {
                    ShipmentNumber = context.Message.ShipmentNumber
                })
                .RequestStarted()
                .TransitionTo(QueryRequest.Pending)
        );


        During(QueryRequest.Pending,
            When(QueryRequest.Completed)
                .Then(context =>
                {
                    context.Saga.LastStatus = context.Message.Status;
                    context.Saga.LastUpdated = context.Message.LastUpdated;
                })
                .RequestCompleted()
                .Schedule(ShipmentStatusExpired, context => new ShipmentStatusExpired()
                {
                    CorrelationId = context.Saga.CorrelationId
                }, _ => TimeSpan.FromSeconds(20))
                .TransitionTo(StatusAvailable),
            
            When(QueryRequest.TimeoutExpired)
                .RequestFaulted(QueryRequest.TimeoutExpired),

            When(QueryRequest.Faulted)
                .RequestFaulted(QueryRequest.Faulted),
            
            When(ShipmentStatusRequested)
                .RequestStarted()
        );

        During(StatusAvailable,
            Ignore(QueryRequest.TimeoutExpired),
            When(ShipmentStatusRequested)
                .Respond(context => new ShipmentStatus()
                {
                    ShipmentNumber = context.Saga.ShipmentNumber,
                    Status = context.Saga.LastStatus,
                    LastUpdated = context.Saga.LastUpdated
                }),
            When(ShipmentStatusExpired.Received)
                .TransitionTo(StatusExpired)
        );
    }

    //
    // ReSharper disable UnassignedGetOnlyAutoProperty
    // ReSharper disable MemberCanBePrivate.Global
    public Event<RequestShipmentStatus> ShipmentStatusRequested { get; } = null!;
    public Request<ShipmentStatusState, QueryShipmentStatus, ShipmentStatus> QueryRequest { get; } = null!;
    public Schedule<ShipmentStatusState, ShipmentStatusExpired> ShipmentStatusExpired { get; } = null!;

    public State StatusAvailable { get; }
    public State StatusExpired { get; }
}