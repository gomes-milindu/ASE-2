using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebApplication1.DTO;
using WebApplication1.Repository.Impl;
using WebApplication1.Repository.Interface;
using WebApplication1.Service.Interface;
using BCrypt.Net;



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

            if (userCheck == null)
            {
                
                return "Please Check Your Username and Password";
            }

            if (userCheck.Username == authLoginDto.username)
            {
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(authLoginDto.password, userCheck.Credential.PasswordHash);

                if (isPasswordValid)
                {
                    
                    return "Login Succesfull";
                }
            }
            

            return "Please Check Your Username and Password";
        }
    }
}
