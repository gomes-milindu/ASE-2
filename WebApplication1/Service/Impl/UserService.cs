
using WebApplication1.DTO;
using WebApplication1.Models;
using WebApplication1.Repository.Interface;
using WebApplication1.Service.Interface;
using BCrypt.Net;

namespace WebApplication1.Service.Impl
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var user = await userRepository.GetUserByEmail(email);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            if(user.Status == AccountStatus.Active)
            {
                throw new Exception("Account is Already active");
            }
            return user;
        }


        public async Task<bool> Register(RegisterUserRequest dto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username.Trim(),
                Status = AccountStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,

                Credential = new UserCredential
                {
                    Id = Guid.NewGuid(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    FailedLoginAttempts = 0,
                    LastPasswordChange = DateTime.UtcNow
                },

                Profile = new UserProfile
                {
                    Id = Guid.NewGuid(),
                    FirstName = dto.FirstName.Trim(),
                    LastName = dto.LastName.Trim(),
                    Email = dto.Email.Trim().ToLower(),
                    PhoneNumber = dto.PhoneNumber,
                    NationalId = dto.NationalId.Trim(),
                    Address = dto.Address
                }
            };

                await userRepository.SaveUser(user);

                 return true;
        }

    };
        

    
}
