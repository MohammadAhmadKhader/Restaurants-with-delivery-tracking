using Shared.Contracts.Attributes;

namespace Auth.Contracts.Dtos.Auth;
public class ForgotPasswordDto
{
    [Masked]
    public string Email { get; set; } = default!;
}