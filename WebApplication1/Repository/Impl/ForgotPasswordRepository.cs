using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace WebApplication1.Repository.Impl
{
    public class ForgotPasswordRepository : IForgotPasswordRepository
    {
        private readonly AppDbContext context;
        public ForgotPasswordRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task AddAsync(ForgotPassword resetTokenEntry)
        {
            await context.ForgotPassword.AddAsync(resetTokenEntry);
            await context.SaveChangesAsync();
        }

        public async Task<int> GetRecentRequestsCountAsync(Guid id, DateTime dateTime)
        {
            return await context.ForgotPassword
                .CountAsync(t =>
                    t.UserId == id &&
                    t.CreatedAt >= dateTime);
        }

        public async Task InvalidatePreviousTokensAsync(Guid id)
        {
            var previousTokens = await context.ForgotPassword
                .Where(t => t.UserId == id && !t.IsUsed)
                .ToListAsync();

            foreach (var token in previousTokens)
            {
                token.IsUsed = true;
            }

            await context.SaveChangesAsync();
        }
    }
}
