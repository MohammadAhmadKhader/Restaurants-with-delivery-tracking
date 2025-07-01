using MassTransit;
using Shared.Kafka;

namespace Restaurants.Sagas;

public class RestaurantSaga : MassTransitStateMachine<RestaurantCreateSagaData>
{
    public State CreatingOwner { get; set; } = default!;
    public State CreatingRestaurant { get; set; } = default!;
    public Event<OwnerCreatedEvent> OwnerCreated { get; set; } = default!;
    public Event<RestaurantCreatedEvent> RestaurantCreated { get; set; } = default!;
    public Event<AcceptedInvitationEvent> AcceptedInvitation { get; set; } = default!;

    public RestaurantSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OwnerCreated, e => e.CorrelateById(m => m.Message.InvitationId));
        Event(() => RestaurantCreated, e => e.CorrelateById(m => m.Message.InvitationId));
        Event(() => AcceptedInvitation, e => e.CorrelateById(m => m.Message.InvitationId));

        Initially(When(AcceptedInvitation).Then(ctx =>
        {
            ctx.Saga.InvitationId = ctx.Message.InvitationId;
        })
        .TransitionTo(CreatingOwner)
        .Produce(x => Task.FromResult(new OwnerCreateCommand(
            x.Message.InvitationId,
            x.Message.Register,
            x.Message.Restaurant))));

        During(CreatingOwner, When(OwnerCreated).Then(ctx =>
        {
            ctx.Saga.OwnerId = ctx.Message.OwnerId;
        })
        .TransitionTo(CreatingRestaurant)
        .Produce(x => Task.FromResult(new RestaurantCreateCommand(
            x.Message.InvitationId,
            x.Message.OwnerId,
            x.Message.Restaurant.Name,
            x.Message.Restaurant.Description,
            x.Message.Restaurant.Phone)))
        );

        During(CreatingRestaurant, When(RestaurantCreated).Then(ctx =>
        {
            ctx.Saga.RestaurantId = ctx.Message.RestaurantId;
        })
        .Finalize());
    }
}