using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebApplication1.DTO;
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
            if (authLoginDto.username == null || authLoginDto.password == null)
            {
                return "Please Check the username or password";
            }

            var userCheck = await userRepository.GetUserByEmail(authLoginDto.username);

            if(userCheck.Profile.Email == authLoginDto.username)
            {
                if(userCheck.Credential.PasswordHash == authLoginDto.password)
                {
                    return "Login Succesfull";
                }

                return "Please Check Your Username and Password Again";
            }
            

            return "Please Check Your Username and Password";
        }
    }
}
