using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Repository.Interface;

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
    }
}
