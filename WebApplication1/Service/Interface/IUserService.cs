using WebApplication1.DTO;
using WebApplication1.Models;

namespace WebApplication1.Service.Interface
{
    public interface IUserService
    {
        Task<IReadOnlyCollection<UserResponseDto>> GetAllUsers();

        Task<User> GetUserByEmail(string email);
        Task<bool> Register(RegisterUserRequest dto);
        Task<UserResponseDto?> SearchUserByEmail(string email);
        Task<bool> SendVerificationEmail(User user);
        Task<bool> SendVerificationSms(User user);
        Task<bool> VerifyEmailOtp(VerifyEmailOtp dto);
        Task<bool> VerifySmsOtp(VerifyEmailOtp dto);
    }
}
