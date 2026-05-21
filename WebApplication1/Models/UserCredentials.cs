using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class UserCredential
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; } // user table eka cnnect wena foreign key

        [Required]
        public string PasswordHash { get; set; }

        public int FailedLoginAttempts { get; set; } = 0;

        public DateTime? LockoutUntil { get; set; }

        public DateTime LastPasswordChange { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
