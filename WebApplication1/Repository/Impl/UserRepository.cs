using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DTO;

namespace WebApplication1.Repository.Impl
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext context;

        public UserRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<IReadOnlyCollection<UserResponseDto>> GetAllUsers()
        {
            List<UserResponseDto> usersList = await context.Users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                Username = u.Username,
                Status = u.Status.ToString(),
                Email = u.Profile.Email,
                FirstName = u.Profile.FirstName,
                LastName = u.Profile.LastName,
                PhoneNumber = u.Profile.PhoneNumber
            })
                .ToListAsync();
            return usersList;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var user = await context.Users.Include(u => u.Profile)
                .Include(u => u.Credential)
                .FirstOrDefaultAsync(u => u.Profile.Email == email);

            return user;
        }

        public async Task<User> GetUserByUsername(string username)
        {
            var user = await context.Users.Include(u => u.Profile)
                .Include(u => u.Credential)
                .FirstOrDefaultAsync(u => u.Username == username);

            return user;
        }

        public async Task SaveUser(User user)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var exists = await context.Users.AnyAsync(u => u.Id == user.Id);

                if (exists)
                {
                    context.Users.Update(user);
                }
                else
                {
                    await context.Users.AddAsync(user);

                }

                await context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public Task UpdateUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}
