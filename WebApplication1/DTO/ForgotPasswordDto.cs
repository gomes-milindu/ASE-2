using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTO
{
    public class ForgotPasswordDto
    {
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    
    }
}
