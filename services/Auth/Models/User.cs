using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

public enum UserRole { CUSTOMER, RESTAURANT_OWNER, DELIVERY_AGENT, ADMIN }
public class User: IdentityUser<Guid>
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required, EmailAddress]
    public override string Email { get; set; }
    public bool IsDeleted { get; set; }
    public IdentityRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<Address> Addresses { get; set; } = new HashSet<Address>();
}
