using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Auth.Utils;
using Restaurants.Contracts.Dtos.Restaurant;

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

    [NotMapped]
    public RestaurantViewDto Restaurant { get; set; } = default!;
    public ICollection<RestaurantPermission> Permissions = new HashSet<RestaurantPermission>();
    public ICollection<User> Users { get; set; } = new HashSet<User>();
}