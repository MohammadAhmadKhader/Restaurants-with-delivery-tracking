using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Auth.Models;
public class User: IdentityUser<Guid>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    [EmailAddress]
    public override string Email { get; set; } = default!;
    // just in case
    [JsonIgnore]
    public override string PasswordHash { get; set; } = default!;

    [Required]
    public bool IsDeleted { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Role> Roles { get; set; } = new HashSet<Role>();
    public ICollection<Address> Addresses { get; set; } = new HashSet<Address>();
}