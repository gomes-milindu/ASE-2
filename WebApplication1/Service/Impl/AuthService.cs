using BCrypt.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Superpower.Parsers;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebApplication1.DTO;
using WebApplication1.DTO;
using WebApplication1.Models;
using WebApplication1.Models.Enums;
using WebApplication1.Repository.Impl;
using WebApplication1.Repository.Interface;
using WebApplication1.Service.Interface;


namespace WebApplication1.Service.Impl
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository userRepository;
        private readonly IConfiguration _config;
        private readonly IEmailService emailService;
        private readonly IForgotPasswordRepository forgotPasswordRepository;


        public AuthService(IUserRepository userRepository, IConfiguration configuration, IForgotPasswordRepository forgotPasswordRepository, IEmailService emailService)
        {
            this.userRepository = userRepository;
            this._config = configuration;
            this.forgotPasswordRepository = forgotPasswordRepository;
            this.emailService = emailService;
            
        }



        public async Task<ForgotPasswordResponseDto> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await userRepository.GetUserByEmail(forgotPasswordDto.Email);

            if(user == null)
            {
                return new ForgotPasswordResponseDto
                {
                    Token = "If the email is registered, a password reset link has been sent."
                };
            }

            var recentRequestsCount = await forgotPasswordRepository.GetRecentRequestsCountAsync(user.Id,DateTime.UtcNow.AddHours(-1));

            if (recentRequestsCount >= 3)
            {
               
                return new ForgotPasswordResponseDto
                {
                    Token = "If the email is registered, a password reset link has been sent."
                };
            }

            // raw token ekak hdnwa email ekata ywnna
            var rawToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            
            // token eka hashn krnwa db eke save krnna
            using var sha256 = SHA256.Create();
            var tokenHash = Convert.ToHexString(sha256.ComputeHash(Encoding.UTF8.GetBytes(rawToken)));

            var resetTokenEntry = new ForgotPassword
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                TokenHash = tokenHash,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15), 
                IsUsed = false,
                AttemptsCount = 0,
                CreatedAt = DateTime.UtcNow
            };

            

            var resetLink = $"https://yourfrontendapp.com/reset-password?token={rawToken}";

            var emailBody = $@"
                            <h3>Password Reset Request</h3>
                            <p>ඔබගේ ගිණුමේ මුරපදය reset කිරීමට ඉල්ලීමක් ලැබී ඇත. පහත link එක click කර නව මුරපදයක් සකසන්න:</p>
                            <p><a href='{resetLink}'>Reset Password</a></p>
                            <p>මෙම link එක වලංගු වන්නේ විනාඩි 15ක් පමණි.</p>";

            await forgotPasswordRepository.InvalidatePreviousTokensAsync(user.Id);

            try
            {
                await emailService.SendEmailAsync(user.Profile.Email, "Reset Your Password", emailBody);

                await forgotPasswordRepository.AddAsync(resetTokenEntry);
            }
            catch (Exception ex)
            {
                
                System.Diagnostics.Debug.WriteLine($"Email send failed: {ex.Message}");

                return new ForgotPasswordResponseDto
                {
                    Token = "Failed to send email. Please try again later."
                };
            }


            return new ForgotPasswordResponseDto
            {
                Token = "Password reset link sent to your email."
            };
        }






        public async Task<AuthLoginResponseDto> Login(AuthLoginDto authLoginDto)
        {
            if (string.IsNullOrEmpty(authLoginDto.username) || string.IsNullOrEmpty(authLoginDto.password))
            {
                return new AuthLoginResponseDto { 
                    Success = false, 
                    Message = "Please check your username and password", 
                    Token = null 
                };
            }

            var userCheck = await userRepository.GetUserByUsername(authLoginDto.username);

            if (userCheck == null)
            {
                return new AuthLoginResponseDto { 
                    Success = false, 
                    Message = "Please Check Your Username and Password", 
                    Token = null 
                };
            }

            if (userCheck.Credential.LockoutUntil.HasValue && userCheck.Credential.LockoutUntil.Value > DateTime.UtcNow)
            {
                return new AuthLoginResponseDto { 
                    Success = false, 
                    Message = "Account Locked", 
                    Token = null 
                };
            }

            if (userCheck.Status != AccountStatus.Active)
            {
                return new AuthLoginResponseDto { 
                    Success = false, 
                    Message = "Please Verify your mobile and email first", 
                    Token = null 
                };
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(authLoginDto.password, userCheck.Credential.PasswordHash);

            if (!isPasswordValid)
            {
                userCheck.Credential.FailedLoginAttempts++;

                if (userCheck.Credential.FailedLoginAttempts >= 5)
                {
                    userCheck.Credential.LockoutUntil = DateTime.UtcNow.AddMinutes(2);
                    await userRepository.SaveUser(userCheck);
                    return new AuthLoginResponseDto { 
                        Success = false, 
                        Message = "Acccount Lock in 5 Minutes", 
                        Token = null 
                    };
                }

                await userRepository.SaveUser(userCheck);
                return new AuthLoginResponseDto { 
                    Success = false, 
                    Message = "Please Check Your Username or Password", 
                    Token = null 
                };
            }

            userCheck.Credential.FailedLoginAttempts = 0;
            userCheck.Credential.LockoutUntil = null;
            await userRepository.SaveUser(userCheck);

            var token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                                        issuer: _config["Jwt:Issuer"],
                                        audience: _config["Jwt:Audience"],
                                        claims: new[]
                                        {
                                    new Claim(ClaimTypes.Name, userCheck.Username),
                                    new Claim(ClaimTypes.NameIdentifier, userCheck.Id.ToString()),
                                    new Claim(ClaimTypes.Role, userCheck.Role.ToString())
                                        },
                                        expires: DateTime.UtcNow.AddHours(1),
                                        signingCredentials: new SigningCredentials(
                                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
                                            SecurityAlgorithms.HmacSha256)
            ));


            

            return new AuthLoginResponseDto { 
                Success = true, 
                Message = "Login Successfull", 
                Token = token 
            };
        }
    }
}
