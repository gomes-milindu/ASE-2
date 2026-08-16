using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTO;
using WebApplication1.Models;
using WebApplication1.Repository.Impl;
using WebApplication1.Service.Interface;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
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

        [HttpPost("SendEmailToVerify")]
        public async Task<IActionResult> SendEmail([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email)) { return BadRequest("Email is required"); }

            var user = await userService.GetUserByEmail(email);


            if (user == null)
            {
                return NotFound("User Not Found");
            }

            var result = await userService.SendVerificationEmail(user);

            if (result)
            {
                return Ok("Verification email sent successfully");
            }
            else
            {
                return StatusCode(500, "Failed to send verification email");
            }

        }

        [HttpPost("VerifyEmailOtp")]
        public async Task<IActionResult> VerifyEmailOtp(VerifyEmailOtp dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await userService.VerifyEmailOtp(dto);
                if (result)
                {
                    return Ok("Email verified successfully");
                }
                else
                {
                    return BadRequest("Invalid OTP");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("SendSmsToVerify")]
        public async Task<IActionResult> SendSms([FromBody] string email)
        {
            if (string.IsNullOrEmpty(email)) { return BadRequest("Email is required"); }
            var user = await userService.GetUserByEmail(email);
            if (user == null)
            {
                return NotFound("User Not Found");
            }
            var result = await userService.SendVerificationSms(user);
            if (result)
            {
                return Ok("Verification SMS sent successfully");
            }
            else
            {
                return StatusCode(500, "Failed to send verification SMS");
            }
        }


        [HttpPost("VerifySmsOtp")]
        public async Task<IActionResult> VerifySmsOtp([FromBody] VerifyEmailOtp dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await userService.VerifySmsOtp(dto);
                if (result)
                {
                    return Ok("Mobile Number verified successfully");
                }
                else
                {
                    return BadRequest("Invalid OTP or User not Found");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }



        [HttpGet("SearchUserByEmail")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await userService.SearchUserByEmail(email);

            return Ok(user);

        }

        [HttpGet("SearchUserByUsername")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var user = await userService.SearchUserByUsername(username);

            return Ok(user);

        }


        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<IReadOnlyCollection<UserResponseDto>>> GetAllUsers()
        {
            var users = await userService.GetAllUsers();
            return Ok(new
            {
                totalCount = users.Count,  
                data = users               
            });
        }


        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(UpdateUserDto updateUser)
        {
            await userService.UpdateUser(updateUser);
            return Ok("User Update successfully");
        }
        

        
    }
}
