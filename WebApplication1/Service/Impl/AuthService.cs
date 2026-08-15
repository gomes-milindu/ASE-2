using BCrypt.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
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

        public async Task<String> Login(AuthLoginDto authLoginDto)
        {
            if (string.IsNullOrEmpty(authLoginDto.username) || string.IsNullOrEmpty(authLoginDto.password))
            {
                return "Please Check the username or password";
            }

            var userCheck = await userRepository.GetUserByUsername(authLoginDto.username);
           

            if (userCheck.Credential.LockoutUntil.HasValue && userCheck.Credential.LockoutUntil.Value > DateTime.UtcNow)
            {
                return "Account Locked";
            }

            if (userCheck.Status != AccountStatus.Active)
            {
                return "Please Verify your mobile and email first";
            }

            if (userCheck == null)
            {
                
                return "Please Check Your Username and Password";
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
                        return "Acccount Lock in 5 Minutes";
                    }

                    await userRepository.SaveUser(userCheck);
                    return "Please Check Your Username or Password";
                }
            }

            userCheck.Credential.FailedLoginAttempts = 0;
            userCheck.Credential.LockoutUntil = null;
            await userRepository.SaveUser(userCheck);

            return "Login Successfull";
        }
    }
}
