using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        public AccountStatus Status { get; set; }

        public virtual UserCredential Credential { get; set; }
        public virtual UserProfile Profile { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }

    public enum AccountStatus
    {
        Pending,
        Active,
        Locked,
        Suspended
    }
}
