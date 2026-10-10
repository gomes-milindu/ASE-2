using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebApplication1.DTO;
using WebApplication1.Service.Interface;


namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly IAuditService auditService;

        private readonly IUserService userService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthController(IAuthService authService , IAuditService auditService, IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            this.authService = authService;
            this.auditService = auditService;
            this.httpContextAccessor = httpContextAccessor;
            this.userService = userService;
        }


        [EnableRateLimiting("LoginPolicy")]
        [HttpPost("loginController")]
        public async Task<IActionResult> Login([FromBody] AuthLoginDto authLoginDto)
        {
            string ipAddress = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown IP";
            var result = await authService.Login(authLoginDto);

            if (string.IsNullOrEmpty(result.Token))
            {
                await auditService.LogActivityAsync(
                    userName: authLoginDto.username,
                    action: "Login Attempt",
                    status: result.Success?"Success":"Failed",
                    ipAddress: ipAddress,
                    details: result.Message
                );

                return Unauthorized(new { message = "Invalid Username or Password" });
            }

            await auditService.LogActivityAsync(
                    userName: authLoginDto.username,
                    action: "Login Attempt",
                    status: result.Success ? "Success" : "Failed",
                    ipAddress: ipAddress,
                    details: result.Message
                );

            return Ok(new
            {
                Message = result.Message,
                Token = result.Token
            });


        }

        [EnableRateLimiting("ForgotPasswordPolicy")]
        [HttpPost("forgotPasswordController")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var forgotPassword = await authService.ForgotPassword(forgotPasswordDto);

            return Ok(new { message = "Password reset link sent to your email" });
        }


    }
}
