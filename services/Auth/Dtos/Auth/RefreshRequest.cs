using System.Text.Json.Serialization;

namespace Auth.Dtos.Auth;

public class RefreshRequest
{
    [JsonPropertyName("refresh-token")]
    public string? RefreshToken { get; set; }
}