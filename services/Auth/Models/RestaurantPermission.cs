using System.ComponentModel.DataAnnotations;

namespace Auth.Models;

public class RestaurantPermission
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string NormalizedName { get; set; } = default!;

    [Required]
    public string DisplayName { get; set; } = default!;
    public bool IsDefaultUser { get; set; }
    public bool IsDefaultAdmin { get; set; }
    public bool IsDefaultOwner { get; set; }
    public ICollection<RestaurantRole> Roles { get; set; } = new HashSet<RestaurantRole>();
}