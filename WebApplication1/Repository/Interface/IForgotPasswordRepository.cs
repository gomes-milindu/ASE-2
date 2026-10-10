using WebApplication1.Models;

namespace WebApplication1.Repository.Interface
{
    public interface IForgotPasswordRepository
    {
        Task AddAsync(ForgotPassword resetTokenEntry);
        Task<int> GetRecentRequestsCountAsync(Guid id, DateTime dateTime);
        Task InvalidatePreviousTokensAsync(Guid id);
    }
}
