using Auth.Contracts.Dtos.Auth;

namespace Shared.Kafka;

public record InvitationAcceptedEvent(Guid InvitationId, Guid RestaurantId, Guid OwnerId, RegisterDto Register);

// TODO: migrate this event to 'TenantOnboardedEvent' or similar
public record OwnerCreatedEvent(Guid InvitationId, Guid OwnerId, Guid RestaurantId);
public record OwnerCreatingFailedEvent(Guid InvitationId);
// public record RestaurantCreatedEvent(Guid InvitationId, Guid OwnerId ,Guid RestaurantId, string Name);
public record RestaurantCreatingFailedEvent(Guid InvitationId, Guid OwnerId);
public record SimpleTestEvent(string Value);