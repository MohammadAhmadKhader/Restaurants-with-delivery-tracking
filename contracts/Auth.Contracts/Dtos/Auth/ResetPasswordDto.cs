namespace Auth.Contracts.Dtos.Auth;

public class ResetPasswordDto(string oldPassword, string newPassword, string confirmNewPassword)
{
    public string OldPassword { get; set; } = oldPassword;
    public string NewPassword { get; set; } = newPassword;
    public string ConfirmNewPassword { get; set; } = confirmNewPassword;
}