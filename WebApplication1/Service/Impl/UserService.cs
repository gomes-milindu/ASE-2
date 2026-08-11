using BCrypt.Net;
using System.Net.NetworkInformation;
using WebApplication1.DTO;
using WebApplication1.Models;
using WebApplication1.Models.Enums;
using WebApplication1.Repository.Interface;
using WebApplication1.Service.Interface;

namespace WebApplication1.Service.Impl
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly IEmailService emailService;
        private readonly ISmsService smsService;
        public UserService(IUserRepository userRepository, IEmailService emailService, ISmsService smsService)
        {
            this.userRepository = userRepository;
            this.emailService = emailService;
            this.smsService = smsService;
        }

        public async Task<bool> SendVerificationEmail(User user)
        {
            string otp = Random.Shared.Next(100000, 999999).ToString();
            string targetEmail = user.Profile.Email;
            string htmlBody = $" <h3>Your otp is 4 is {otp}</h3>";

            try
            {
                
                user.Credential.VerificationOtp = otp;
                user.Credential.OtpGeneratedAt = DateTime.UtcNow;
                user.Credential.OtpExpiredAt = DateTime.UtcNow.AddMinutes(5);

                await userRepository.SaveUser(user);

                await emailService.SendEmailAsync(targetEmail, "Account Verification", htmlBody);
                
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            
            
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var user = await userRepository.GetUserByEmail(email);
            if (user == null)
            {
                throw new ArgumentNullException("User not found");
               
            }

            if (user.Status == AccountStatus.Active)
            {
                throw new Exception("Account is Already active");
            }
            return user;
        }


        public async Task<bool> Register(RegisterUserRequest dto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username.Trim(),
                Status = AccountStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,

                Credential = new UserCredential
                {
                    Id = Guid.NewGuid(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    FailedLoginAttempts = 0,
                    LastPasswordChange = DateTime.UtcNow
                },

                Profile = new UserProfile
                {
                    Id = Guid.NewGuid(),
                    FirstName = dto.FirstName.Trim(),
                    LastName = dto.LastName.Trim(),
                    Email = dto.Email.Trim().ToLower(),
                    PhoneNumber = dto.PhoneNumber,
                    NationalId = dto.NationalId.Trim(),
                    Address = dto.Address
                }
            };

                await userRepository.SaveUser(user);

                 return true;
        }

        public async Task<bool> VerifyEmailOtp(VerifyEmailOtp dto)
        {
            var user = await userRepository.GetUserByEmail(dto.Email);
            if (user == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(user.Credential.VerificationOtp))
            {
                return false;
            }

            if (DateTime.UtcNow > user.Credential.OtpExpiredAt)
            {
                throw new TimeoutException("Your Email Otp Time Out. Try Again");
            }

            if (user.Credential.VerificationOtp == dto.Otp)
            {
                

                user.Credential.VerificationOtp = null;
                user.Credential.OtpGeneratedAt = null;
                user.Credential.OtpExpiredAt = null;

                await userRepository.SaveUser(user);
                return true;
            }
            return false;
        }

        public async Task<bool> SendVerificationSms(User user)
        {
            string otp = Random.Shared.Next(100000, 999999).ToString();
            string targetPhoneNumber = user.Profile.PhoneNumber;

            string smsBody = $"Your OTP is {otp}";

            try
            {
                user.Credential.MobileVerificationOtp = otp;
                user.Credential.MobileOtpGeneratedAt = DateTime.UtcNow;
                user.Credential.MobileOtpExpiresAt = DateTime.UtcNow.AddMinutes(5);

                await userRepository.SaveUser(user);
                await smsService.SendSmsAsync(targetPhoneNumber, smsBody);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

        }

        public async Task<bool> VerifySmsOtp(VerifyEmailOtp dto)
        {
            var user = await userRepository.GetUserByEmail(dto.Email);
            if (user == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(user.Credential.MobileVerificationOtp))
            {
                return false;
            }

            if (DateTime.UtcNow > user.Credential.MobileOtpExpiresAt)
            {
                throw new TimeoutException("Your Sms Otp Time Out. Try Again");
            }

            if (user.Credential.MobileVerificationOtp == dto.Otp)
            {
                

                user.Credential.MobileVerificationOtp = null;
                user.Credential.MobileOtpGeneratedAt = null;
                user.Credential.MobileOtpExpiresAt = null;

                if (string.IsNullOrEmpty(user.Credential.VerificationOtp))
                {
                    user.Status = AccountStatus.Active; 
                }

                await userRepository.SaveUser(user);
                return true;
            }
            return false;
        }

        public async Task<UserResponseDto?> SearchUserByEmail(string email)
        {
            if(email == null)
            {
                throw new ArgumentNullException(nameof(email), "Email cannot be null");
            }

            var user = await userRepository.GetUserByEmail(email);
            if(user== null) return null;

            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Status = user.Status.ToString(),
                Email = user.Profile?.Email ?? string.Empty,
                FirstName = user.Profile?.FirstName ?? string.Empty,
                LastName = user.Profile?.LastName ?? string.Empty,
                PhoneNumber = user.Profile?.PhoneNumber ?? string.Empty
            };
        }

        public async Task<IReadOnlyCollection<UserResponseDto>> GetAllUsers()
        {
            var users = await userRepository.GetAllUsers();
            return users;
        }

        public async Task<bool> UpdateUser(UpdateUserDto updateUser)
        {

            System.Diagnostics.Debug.WriteLine($"Update User Email: {updateUser.Email}");
            var updateUserEmail =await  userRepository.GetUserByEmail(updateUser.Email);
            if (updateUserEmail == null)
            {
                return false;
            }

           

            return true;
        }

        public async Task<UserResponseDto?> SearchUserByUsername(string username)
        {
            if (username == null)
            {
                throw new ArgumentNullException(nameof(username), "Username cannot be null");
            }

            var user = await userRepository.GetUserByUsername(username);
            if(user == null) return null;

            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Status = user.Status.ToString(),
                Email = user.Profile?.Email ?? string.Empty,
                FirstName = user.Profile?.FirstName ?? string.Empty,
                LastName = user.Profile?.LastName ?? string.Empty,
                PhoneNumber = user.Profile?.PhoneNumber ?? string.Empty
            };
        }
    };
        

    
}
