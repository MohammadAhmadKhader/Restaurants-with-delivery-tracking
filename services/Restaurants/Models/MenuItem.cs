using System.ComponentModel.DataAnnotations;
using Restaurants.Utils;

namespace Restaurants.Models;
public class MenuItem: ITenantModel
{
    [Key]
    public int Id { get; set; }
    public Guid RestaurantId { get; set; }
    public string Name { get; set; } = default!;
    public string NormalizedName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public string ImageUrl { get; set; } = default!;
    public ICollection<Menu> Menus { get; set; } = new HashSet<Menu>();
}