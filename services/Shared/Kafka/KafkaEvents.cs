using Auth.Contracts.Dtos.Auth;
using Restaurants.Contracts.Dtos;

namespace Shared.Kafka;

public record InvitationAcceptedEvent(Guid InvitationId, RestaurantCreateDto Restaurant, RegisterDto Register);
public record OwnerCreatedEvent(Guid InvitationId, Guid OwnerId, RestaurantCreateDto Restaurant);
public record OwnerCreatingFailedEvent(Guid InvitationId);
public record RestaurantCreatedEvent(Guid InvitationId, Guid OwnerId ,Guid RestaurantId, string Name);
public record RestaurantCreatingFailedEvent(Guid InvitationId, Guid OwnerId);
public record SimpleTestEvent(string Value);