using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTO;

namespace WebApplication1.Service.Interface
{
    public interface IAuthService
    {
        Task<AuthLoginResponseDto> Login(AuthLoginDto authLoginDto);
    }
}
