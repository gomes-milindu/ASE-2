using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTO
{
    public class VerifyEmailOtp
    {
        [Required (ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        
        [Required (ErrorMessage = "OTP is required.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP should be 6 digits")]
        public string Otp { get; set; }
    }
}
