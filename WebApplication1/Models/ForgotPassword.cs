namespace WebApplication1.Models
{
    public class ForgotPassword
    {
        public string Id { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public int AttemptsCount { get; set; }
        public string? RequestedIp { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Property (User table එකට link වෙන්න)
        public virtual User User { get; set; } = null!;
    }
}
