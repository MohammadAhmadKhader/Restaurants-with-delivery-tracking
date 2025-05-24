using System.ComponentModel.DataAnnotations;

namespace Auth.Models;

public class Permission
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public required string Name { get; set; }

    [Required]
    public required string DisplayName { get; set; }

    [Required]
    public required bool IsDefaultUser { get; set; }

    [Required]
    public required bool IsDefaultAdmin { get; set; }

    [Required]
    public required bool IsDefaultSuperAdmin { get; set; }
    public ICollection<Role> Roles { get; set; } = new HashSet<Role>();
}