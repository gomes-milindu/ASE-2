using System.ComponentModel.DataAnnotations;
using WebApplication1.DTO;
using WebApplication1.Models.Enums;

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
        public AccountStatus Status { get; set; } = AccountStatus.Pending;

        public UserRole Role { get; set; } = UserRole.Customer;

        public virtual UserCredential Credential { get; set; }
        public virtual UserProfile Profile { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }

   
}
