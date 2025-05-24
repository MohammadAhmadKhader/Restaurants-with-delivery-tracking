using System.ComponentModel.DataAnnotations;
using FoodDelivery.Protos.Auth;
using Microsoft.AspNetCore.Identity;

namespace Auth.Models;

public class Role : IdentityRole<Guid>
{
    [Required]
    public required string DisplayName { get; set; }

    [Required]
    public override string Name { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Permission> Permissions { get; set; } = new HashSet<Permission>();
    public ICollection<User> Users { get; set; } = new HashSet<User>();
}