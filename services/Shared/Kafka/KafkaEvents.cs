using Auth.Contracts.Dtos.Auth;
using Restaurants.Contracts.Dtos;

namespace Shared.Kafka;

public record AcceptedInvitationEvent(Guid InvitationId, RestaurantCreateDto Restaurant, RegisterDto Register);
public record OwnerCreatedEvent(Guid InvitationId, Guid OwnerId, RestaurantCreateDto Restaurant);
public record OwnerCreatingFailed(Guid InvitationId);
public record RestaurantCreatedEvent(Guid InvitationId, Guid RestaurantId, string Name);
public record RestaurantCreatingFailed(Guid InvitationId);

public record SimpleTestEvent(string value);