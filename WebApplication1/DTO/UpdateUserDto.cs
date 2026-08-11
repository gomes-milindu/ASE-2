using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTO
{
    public class UpdateUserDto
    {
        
        public string Password { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        public string PhoneNumber { get; set; }
        public string NationalId { get; set; }
        public string Address { get; set; }
    }
}
