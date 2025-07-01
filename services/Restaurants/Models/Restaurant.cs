using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurants.Models;

public class Restaurant
{
    [Key]
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public Guid AddressId { get; set; }
    public bool IsOpen { get; set; }
    [NotMapped]
    public decimal Rating { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Menu> Menus { get; set; } = new HashSet<Menu>();
}