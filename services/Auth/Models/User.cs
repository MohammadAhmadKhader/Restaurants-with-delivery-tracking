using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Auth.Utils;
using Microsoft.AspNetCore.Identity;
using Shared.Data.Patterns.AuditTimestamp;

namespace Auth.Models;

public enum DefaultUserRoles { None, User, Admin, SuperAdmin }
public enum DefaultRestaurantUserRoles { None, Customer, Admin, Owner }
public enum DeleteUserError { None, NotFound, ForbiddenAdmin, ForbiddenOwner, Unexpected }
public class User : IdentityUser<Guid>, IUpdatedAt
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
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? RestaurantId { get; set; }
    public bool IsGlobal { get; set; }
    public ICollection<Role> Roles { get; set; } = new HashSet<Role>();
    public ICollection<Address> Addresses { get; set; } = new HashSet<Address>();
    public ICollection<RestaurantRole> RestaurantRoles { get; set; } = new HashSet<RestaurantRole>();
}