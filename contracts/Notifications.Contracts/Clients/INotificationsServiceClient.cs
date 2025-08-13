using Notifications.Contracts.Dtos;
using Refit;

namespace Notifications.Contracts.Clients;

public interface INotificationsServiceClient
{
    [Post("/api/notifications/forgot-password")]
    Task<ApiResponse<object>> SendForgotPasswordEmail([Body] SendForgotPasswordEmailDto dto);
}