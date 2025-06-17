using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Auth.Utils;
using Microsoft.AspNetCore.Identity;

namespace Auth.Models;

public enum DefaultUserRoles { None, User, Admin, SuperAdmin }
public enum DeleteUserError { None, NotFound, ForbiddenAdmin, ForbiddenOwner, Unexpected }
public class User : IdentityUser<Guid>
{
    [MaxLength(Constants.MaxFirstNameLength)]
    public string? FirstName { get; set; } = default!;

    [MaxLength(Constants.MaxLastNameLength)]
    public string? LastName { get; set; } = default!;

    [EmailAddress]
    [MaxLength(Constants.MaxEmailLength)]
    public override string? Email { get; set; } = default!;
    // just in case
    [JsonIgnore]
    public override string? PasswordHash { get; set; } = default!;

    [Required]
    public bool IsDeleted { get; set; }
    // public Guid TenantId { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Role> Roles { get; set; } = new HashSet<Role>();
    public ICollection<Address> Addresses { get; set; } = new HashSet<Address>();
}