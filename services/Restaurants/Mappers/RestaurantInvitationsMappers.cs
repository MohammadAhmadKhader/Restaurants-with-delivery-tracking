using Restaurants.Contracts.Dtos;
using Restaurants.Models;

namespace Restaurants.Mappers;

public static class RestaurantInvitationsMappers
{
    public static RestaurantInvitationViewDto ToViewDto(this RestaurantInvitation invitation)
    {
        return new RestaurantInvitationViewDto
        {
            Id = invitation.Id,
            InvitedEmail = invitation.InvitedEmail,
            ExpiresAt = invitation.ExpiresAt,
            UsedAt = invitation.UsedAt,
        };
    }
}