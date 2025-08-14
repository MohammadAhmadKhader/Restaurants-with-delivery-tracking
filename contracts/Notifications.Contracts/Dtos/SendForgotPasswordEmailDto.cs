using Shared.Contracts.Attributes;

namespace Notifications.Contracts.Dtos;

public record SendForgotPasswordEmailDto([Masked] string Email, string Token);