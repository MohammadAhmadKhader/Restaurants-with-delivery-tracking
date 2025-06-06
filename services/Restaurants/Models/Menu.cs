
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Menu
{
    [Key]
    public int Id { get; set; }
    public int RestaurantId { get; set; }
    public string Category { get; set; }
    public string Name { get; set; }
    
    [ForeignKey(nameof(RestaurantId))]
    public Restaurant Restaurant { get; set; }
    public ICollection<MenuItem> Items { get; set; } = new HashSet<MenuItem>();
}