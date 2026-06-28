using WebApplication1.DTO;
using WebApplication1.Models;

namespace WebApplication1.Service.Interface
{
    public interface IUserService
    {
        Task<User> GetUserByEmail(string email);
        Task<bool> Register(RegisterUserRequest dto);
        Task<bool> SendVerificationEmail(User user);
        Task<bool> SendVerificationSms(User user);
        Task<bool> VerifyEmailOtp(VerifyEmailOtp dto);
    }
}
