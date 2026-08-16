using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Superpower.Model;
using System.Text.Json;
using WebApplication1.DTO;
using WebApplication1.Service.Impl;
using WebApplication1.Service.Interface;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly IAuditService auditService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthController(IAuthService authService , IAuditService auditService, IHttpContextAccessor httpContextAccessor)
        {
            this.authService = authService;
            this.auditService = auditService;
            this.httpContextAccessor = httpContextAccessor;
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

    }
}
