namespace Notifications.Contracts.Dtos;

public record SendForgotPasswordEmailDto(string Email, string Token);