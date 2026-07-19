using WebApplication1.DTO;
using WebApplication1.Models;

namespace WebApplication1.Repository.Interface
{
    public interface IUserRepository
    {
        Task<IReadOnlyCollection<UserResponseDto>> GetAllUsers();
        Task<User> GetUserByEmail(string email);
        Task SaveUser(User user);
        
        Task UpdateUser(User user);
    }
}
