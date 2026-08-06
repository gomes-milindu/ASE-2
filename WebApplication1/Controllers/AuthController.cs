using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebApplication1.DTO;
using WebApplication1.Service.Interface;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("loginController")]
        public async Task<IActionResult> Login([FromBody] AuthLoginDto authLoginDto)
        {
            var token = await authService.Login(authLoginDto);
           
            return Ok(new
            {
                Message = "Login successful.",
                Token = token
            });


        }

    }
}
