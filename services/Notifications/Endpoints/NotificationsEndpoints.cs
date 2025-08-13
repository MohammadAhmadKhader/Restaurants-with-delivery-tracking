using Notifications.Contracts.Dtos;
using Notifications.Services.IServices;

namespace Notifications.Endpoints;

public static class NotificationsEndpoints
{
    public static void MapNotificationsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/notifications");

        group.MapPost("/forgot-password", async (SendForgotPasswordEmailDto dto, IEmailService emailService) =>
        {
            await emailService.SendForgotPassword(dto.Email, dto.Token);
        });
    }
}