using System.Text.Json.Serialization;

namespace Auth.Contracts.Dtos.Auth;

public class RefreshRequest
{
    [JsonPropertyName("refresh-token")]
    public string? RefreshToken { get; set; }
}