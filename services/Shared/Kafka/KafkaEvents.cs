using Auth.Contracts.Dtos.Auth;

namespace Shared.Kafka;
public record RestaurantCreatedEvent(Guid InvitationId, Guid RestaurantId, Guid OwnerId, RegisterDto Register);
public record OwnerCreatedEvent(Guid InvitationId, Guid OwnerId, Guid RestaurantId);
public record OwnerCreatingFailedEvent(Guid InvitationId, Guid RestaurantId);
public record SimpleTestEvent(string Value);
public record OrderCheckoutCompleted(Guid OrderId, Guid RestaurantId, Guid UserId);