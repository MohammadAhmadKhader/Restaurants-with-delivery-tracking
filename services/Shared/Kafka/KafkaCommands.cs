using Auth.Contracts.Dtos.Auth;
using Restaurants.Contracts.Dtos;

namespace Shared.Kafka;

public record OwnerCreateCommand(Guid InvitationId, RegisterDto RegisterDto, RestaurantCreateDto RestaurantCreateDto);
public record RestaurantCreateCommand(Guid InvitationId, Guid OwnerId, string Name, string? Description, string? Phone);