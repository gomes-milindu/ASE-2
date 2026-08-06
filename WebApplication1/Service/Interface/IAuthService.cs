using WebApplication1.DTO;

namespace WebApplication1.Service.Interface
{
    public interface IAuthService
    {
        Task<String> Login(AuthLoginDto authLoginDto);
    }
}
