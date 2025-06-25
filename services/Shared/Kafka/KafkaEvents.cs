using Auth.Contracts.Dtos.Auth;
using Restaurants.Contracts.Dtos;

namespace Shared.Kafka;

public record AcceptedInvitationEvent(Guid InvitationId, RestaurantCreateDto Restaurant, RegisterDto Register);
public record OwnerCreatedEvent(Guid OwnerId, RestaurantCreateDto Restaurant);
public record RestaurantCreatedEvent(Guid RestaurantId, string Name);

public record SimpleTestEvent(string value);