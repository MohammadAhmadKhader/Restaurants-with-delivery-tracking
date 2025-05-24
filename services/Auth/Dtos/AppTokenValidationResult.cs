using System.Security.Claims;

namespace Auth.Dtos;
public class AppTokenValidationResult
{
    public bool IsValid { get; set; }
    public ClaimsPrincipal? Principal { get; set; }
    public string? Error { get; set; }
}