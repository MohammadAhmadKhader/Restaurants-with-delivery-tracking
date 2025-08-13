using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Restaurants.Utils;

namespace Restaurants.Models;

public class Restaurant
{
    [Key]
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }

    [MaxLength(Constants.MaxRestaurantNameLength)]
    public string Name { get; set; } = default!;

    [MaxLength(Constants.MaxRestaurantDescriptionLength)]
    public string Description { get; set; } = default!;

    [Phone]
    public string Phone { get; set; } = default!;
    public Guid AddressId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Menu> Menus { get; set; } = new HashSet<Menu>();
}