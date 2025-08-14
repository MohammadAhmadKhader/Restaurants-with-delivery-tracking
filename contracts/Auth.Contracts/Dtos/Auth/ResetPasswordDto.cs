using Shared.Contracts.Attributes;

namespace Auth.Contracts.Dtos.Auth;

public class ResetPasswordDto(string oldPassword, string newPassword, string confirmNewPassword)
{
    [Masked]
    public string OldPassword { get; set; } = oldPassword;

    [Masked]
    public string NewPassword { get; set; } = newPassword;

    [Masked]
    public string ConfirmNewPassword { get; set; } = confirmNewPassword;
}