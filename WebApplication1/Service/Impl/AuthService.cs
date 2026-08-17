using BCrypt.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Superpower.Parsers;
using System.Diagnostics;
using System.Security.Claims;
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

        public AuthService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
            
        }

        public async Task<AuthLoginResponseDto> Login(AuthLoginDto authLoginDto)
        {
            if (string.IsNullOrEmpty(authLoginDto.username) || string.IsNullOrEmpty(authLoginDto.password))
            {
                return new AuthLoginResponseDto
                {
                    Success = false,
                    Message = "Please check your username and password",
                    Token = null
                };
            }

            var userCheck = await userRepository.GetUserByUsername(authLoginDto.username);
           

            if (userCheck.Credential.LockoutUntil.HasValue && userCheck.Credential.LockoutUntil.Value > DateTime.UtcNow)
            {
                return new AuthLoginResponseDto
                {
                    Success = false,
                    Message = "Account Locked",
                    Token = null
                };
            }

            if (userCheck.Status != AccountStatus.Active)
            {
                // return "Please Verify your mobile and email first";
                return new AuthLoginResponseDto
                {
                    Success = false,
                    Message = "Please Verify your mobile and email first",
                    Token = null
                };
            }

            if (userCheck == null)
            {
                return new AuthLoginResponseDto
                {
                    Success = false,
                    Message = "Please Check Your Username and Password",
                    Token = null
                };
            }

            if (userCheck.Username == authLoginDto.username)
            {
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(authLoginDto.password, userCheck.Credential.PasswordHash);

                if (!isPasswordValid)
                {
                    
                    userCheck.Credential.FailedLoginAttempts++;

                   
                    if (userCheck.Credential.FailedLoginAttempts >= 5)
                    {
                        userCheck.Credential.LockoutUntil = DateTime.UtcNow.AddMinutes(2);
                        await userRepository.SaveUser(userCheck);
                        // return "Acccount Lock in 5 Minutes";
                        return new AuthLoginResponseDto
                        {
                            Success = false,
                            Message = "Acccount Lock in 5 Minutes",
                            Token = null
                        };
                    }

                    await userRepository.SaveUser(userCheck);
                    //return "Please Check Your Username or Password";
                    return new AuthLoginResponseDto
                    {
                        Success = false,
                        Message = "Please Check Your Username or Password",
                        Token = null
                    };

                }
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
            return new AuthLoginResponseDto
            {
                Success = true,
                Message = "Login Successfull",
                Token = "nihjl458/ibhfgKKL:kmlm698lknn ,mnl;"
            };
        }
    }
}
