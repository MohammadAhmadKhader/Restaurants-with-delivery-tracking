
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Restaurants.Utils;

namespace Restaurants.Models;
public class Menu: ITenantModel
{
    [Key]
    public int Id { get; set; }
    public Guid RestaurantId { get; set; }

    [MaxLength(Constants.MaxMenuCategoryLength)]
    public string Category { get; set; } = default!;

    [MaxLength(Constants.MaxMenuNameLength)]
    public string Name { get; set; } = default!;
    public string NormalizedName { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(RestaurantId))]
    public Restaurant? Restaurant { get; set; }
    public ICollection<MenuItem> Items { get; set; } = new HashSet<MenuItem>();
}