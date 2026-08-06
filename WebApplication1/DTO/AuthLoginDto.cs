using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTO
{
    public class AuthLoginDto
    {

        [Required(ErrorMessage = "Username is Required.")]
        public string username { get; set; }

        [Required(ErrorMessage = "Password is Required.")]
        public string password { get; set; }
    }
}
