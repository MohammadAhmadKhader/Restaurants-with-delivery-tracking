using System.ComponentModel.DataAnnotations;
using Auth.Utils;
using Restaurants.Contracts.Dtos;

namespace Auth.Models;

public class RestaurantRole
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(Constants.MaxRoleNameLength)]
    public string NormalizedName { get; set; } = default!;

    [Required]
    [MaxLength(Constants.MaxRoleNameLength)]
    public string DisplayName { get; set; } = default!;

    [Required]
    public Guid RestaurantId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public RestaurantViewDto Restaurant { get; set; } = default!;
    public ICollection<RestaurantPermission> Permissions = new HashSet<RestaurantPermission>();
    public ICollection<User> Users { get; set; } = new HashSet<User>();
}