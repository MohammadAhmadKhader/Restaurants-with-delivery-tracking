using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurants.Models;
public class MenuItem
{
    [Key]
    public int Id { get; set; }
    public int MenuId { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public string ImageUrl { get; set; } = default!;

    [ForeignKey(nameof(MenuId))]
    public Menu? Menu { get; set; }
}