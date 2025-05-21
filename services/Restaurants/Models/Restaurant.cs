using System.ComponentModel.DataAnnotations;

public class Restaurant
{
    [Key]
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Phone { get; set; }
    public Guid AddressId { get; set; }
    public bool IsOpen { get; set; }
    public decimal Rating { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Menu> Menus { get; set; } = new HashSet<Menu>();
}