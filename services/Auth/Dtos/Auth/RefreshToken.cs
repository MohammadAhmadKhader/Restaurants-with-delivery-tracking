namespace Auth.Dtos.Auth;
public class RefreshToken
{
    public string Token { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}
