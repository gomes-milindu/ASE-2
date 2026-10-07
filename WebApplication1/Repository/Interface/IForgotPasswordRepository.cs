using WebApplication1.Models;

namespace WebApplication1.Repository.Interface
{
    public interface IForgotPasswordRepository
    {
        Task AddAsync(ForgotPassword resetTokenEntry);
    }
}
