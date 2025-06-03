using System.ComponentModel.DataAnnotations;

namespace Auth.Models;

public class Permission
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public string DisplayName { get; set; } = default!;

    [Required]
    public bool IsDefaultUser { get; set; } = default!;

    [Required]
    public bool IsDefaultAdmin { get; set; } = default!;

    [Required]
    public bool IsDefaultSuperAdmin { get; set; } = default!;
    public ICollection<Role> Roles { get; set; } = new HashSet<Role>();
}