using MassTransit;

namespace Restaurants.Sagas;

public class RestaurantCreateSagaData : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public Guid RestaurantId { get; set; }
    public Guid OwnerId { get; set; }
    public Guid InvitationId { get; set; }
    public string CurrentState { get; set; } = default!;
}