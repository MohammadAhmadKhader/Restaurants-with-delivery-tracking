using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auth.Models;

public class PasswordResetToken
{
    [Key]
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public string Token { get; set; } = default!;
    public bool IsUsed { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}
