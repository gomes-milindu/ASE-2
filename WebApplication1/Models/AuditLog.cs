using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models  
{
    public class AuditLog
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? UserName { get; set; }
        public string Action { get; set; }
        public string Status { get; set; }
        public string IpAddress { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        public string? Details { get; set; }



    }
}
