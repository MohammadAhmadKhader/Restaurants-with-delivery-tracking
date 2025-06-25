namespace Shared.Kafka;

public record OwnerCreateCommand(Guid InvitationId, string FirstName, string LastName, string Email, string Password);
public record RestaurantCreateCommand(Guid OwnerId, string Name, string? Description, string? Phone);