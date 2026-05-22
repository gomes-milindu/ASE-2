using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTO;
using WebApplication1.Models;
using WebApplication1.Service.Interface;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService userService;


        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserRequest dto)
        {
            await userService.Register(dto);

            return Ok("User registered successfully");
        }

        [HttpPost("emailVerify")]
        public async Task<IActionResult> SendEmail([FromBody] string email)
        {
            if(string.IsNullOrEmpty(email)) { return BadRequest("Email is required"); }

            var user = await userService.GetUserByEmail(email);
            await userService.SendVerificationEmail(user);


            return Ok("User Found: " + user.Username);

        }
    }
}
