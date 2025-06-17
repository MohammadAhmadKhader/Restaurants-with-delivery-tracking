
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurants.Models;
public class Menu
{
    [Key]
    public int Id { get; set; }
    public int RestaurantId { get; set; }
    public string Category { get; set; } = default!;
    public string Name { get; set; } = default!;

    [ForeignKey(nameof(RestaurantId))]
    public Restaurant? Restaurant { get; set; }
    public ICollection<MenuItem> Items { get; set; } = new HashSet<MenuItem>();
}