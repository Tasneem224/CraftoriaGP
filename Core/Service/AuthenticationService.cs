using DomainLayer.Contracts;
    using DomainLayer.Exceptions;
    using DomainLayer.Exceptions.DomainLayer.Exceptions;
    using DomainLayer.Models.Identity;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using ServiceAbstraction;
    using Shared.IdentityModule;
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
using Google.Apis.Auth;
using static System.Net.WebRequestMethods;

namespace Service
    {
        public class AuthenticationService(UserManager<ApplicationUser> _userManager, IConfiguration _configuration, ICloudinaryService _cloudinary, IEmailService _emailService,IEmailVerificationCodeRepository _emailVerificationRepo) : IAuthenticationService
        {
          
            public async Task<ReturnUserDTO> RegisterAsync(RegisterDto _registerDto)
            {
                string? profileImagePath = null;

                string roleName = _registerDto.Role.ToString();
                string gender = _registerDto.Gender.ToString();
                string portfolioPath = null;

                try
                {
                   
                    var existingUser = await _userManager.FindByEmailAsync(_registerDto.Email);
                    if (existingUser is not null)
                    {
                        throw new UserAlreadyExistsException(_registerDto.Email);
                    }

                    profileImagePath = _registerDto.ProfileImage is not null ? await _cloudinary.UploadAsync(_registerDto.ProfileImage) : null;

                    portfolioPath = await ExpertOption(_registerDto, portfolioPath);

                    var user = CreatingUserName(_registerDto.Email);
                    var newUser = new ApplicationUser
                    {
                        UserName = user,
                        DisplayName = user,
                        Email = _registerDto.Email,
                        FirstName = _registerDto.FirstName,
                        SecondName = _registerDto.LastName,
                        NormalizedEmail = _registerDto.Email,
                        ProfileImage = profileImagePath,
                        Portfolio = portfolioPath,
                        Gender = (Gender)Enum.Parse(typeof(Gender), gender),
                        YearsOfExperience = _registerDto.YearsOfExperience,
                  


                    };

                    var result = await _userManager.CreateAsync(newUser, _registerDto.Password);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(newUser, roleName);
                        return new ReturnUserDTO
                        {
                            Email = newUser.Email,
                            UserName = newUser.UserName,
                            Token = await CreateTokenAsync(newUser)
                        };
                    }
                    else
                    {
                        throw new DomainLayer.Exceptions.InvalidOperationExceptionCustome(result.Errors.Select(e => e.Description).ToList());
                    }

                }
                catch (Exception)
                {
                    exceptionConditionForProfileAndPortfolio(profileImagePath, portfolioPath);

                    throw;

                }
            }
            public async Task<ReturnUserDTO> LoginAsync(LoginDTO loginDto)
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user is null)
                {
                    throw new UserNotFoundException(loginDto.Email);
                }
                //Check passwords match
                var checkPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);
                if (checkPassword)
                {
                    return new ReturnUserDTO
                    {
                        Email = user.Email,
                        UserName = user.DisplayName,
                        Token = await CreateTokenAsync(user)

                    };
                }
                throw new UnauthorizedAException();
            }
            public async Task<string> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {

                throw new UserNotFoundException(email);
            }

            // Generate OTP
            string otp = new Random().Next(100000, 999999).ToString();

            // Update User Properties
            user.OtpCode = otp;
            user.OtpExpiration = DateTime.UtcNow.AddMinutes(10);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception("Could not save OTP");
            }

            // Send Email
            await _emailService.SendEmailAsync(email, "Reset Password OTP", $"Your OTP is: {otp}");

            return "OTP sent successfully.";
        }
            public async Task<string> ResetPasswordAsync(ResetPasswordDto model)
        { 
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) throw new UserNotFoundException(model.Email);

            if (user.OtpCode != model.OtpCode || user.OtpExpiration < DateTime.UtcNow)
            {
                throw new InvalidException("Invalid or Expired OTP");
            }

            
                if (await _userManager.HasPasswordAsync(user))
                {
                    await _userManager.RemovePasswordAsync(user);
                }

                var result = await _userManager.AddPasswordAsync(user, model.NewPassword);

                if (!result.Succeeded)
                {
           
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationExceptionCustome($"Failed to reset password: {errors}");
                }

                user.OtpCode = null;
                user.OtpExpiration = null;

                await _userManager.UpdateAsync(user);

                return "Password has been reset";
            }
            public async Task<bool> VerifyOtpAsync(VerifyOtpDto model)
                 {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user == null) return false;

                    if (user.OtpCode == model.OtpCode && user.OtpExpiration > DateTime.UtcNow)
                    {
                        return true;
                    }

                    return false;
                }
            public async Task<ReturnEmailOTP> VerifyEmailAsync(VerifyEmailDTO email)
            {
                if (email is null)
                    throw new InvalidException("Email cannot be null.");

                await _emailVerificationRepo.MarkOldOtpsAsync(email.Email);

                var otpGenerated = GenerateOTP();

                await _emailVerificationRepo.CreateOtpAsync(email.Email, otpGenerated, DateTime.UtcNow.AddMinutes(5));

                await SendOtpEmailAsync(email.Email, otpGenerated);

                return new ReturnEmailOTP
                {
                    Email = email.Email,
                    OtpCode = otpGenerated
                };
            }
            public async Task<bool> CheckEmailOTPAsync(VerifyOtpDto emailOTP)
                {
                    if(emailOTP.Email is null || emailOTP.OtpCode is null)
                        throw new InvalidException("Email and OTP code cannot be null.");

                            var activeOtp = await _emailVerificationRepo.GetActiveOtpByEmailAsync(emailOTP.Email);
                            if (activeOtp is null)
                                throw new InvalidException("OTP expired, used, or invalid. Please request a new one.");

                            return await _emailVerificationRepo.VerifyOtpAsync(emailOTP.Email, emailOTP.OtpCode);
            }


            private void exceptionConditionForProfileAndPortfolio(string? profileImagePath, string portfolioPath)
        {
            if (profileImagePath is not null)
            {
                _cloudinary.DeleteAsync(profileImagePath);
            }

            if (portfolioPath is not null)
            {
                _cloudinary.DeleteAsync(portfolioPath);
            }
        }
            private async Task<string> ExpertOption(RegisterDto _registerDto, string portfolioPath)
            {
                if (_registerDto.Role == RoleType.Expert)
                {

                    if (_registerDto.Portfolio is null)
                    {
                        throw new InvalidOperationExceptionCustome(new List<string> { "Expert registration requires Portfolio." });
                    }

                    portfolioPath = await _cloudinary.UploadAsync(_registerDto.Portfolio);
                }

                return (portfolioPath);
            }
            private async Task<string> CreateTokenAsync(ApplicationUser user)
            {
                var claims = new List<Claim>()
                {

                    new Claim(ClaimTypes.Email, user.Email!),
                    new Claim(ClaimTypes.Name,user.UserName!),
                    new Claim(ClaimTypes.NameIdentifier,user.Id!),
                };
                var roles = await _userManager.GetRolesAsync(user);
                    foreach (var role in roles)

                    claims.Add(new Claim(ClaimTypes.Role, role));
                var secretKey = _configuration.GetSection("JWTOptions")["secretKey"];
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWTOptions:issuer"],
                    audience: _configuration["JWTOptions:audience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: creds
                    );
                return new JwtSecurityTokenHandler().WriteToken(token);


            }
            private string BuildOtpEmailBody(   string otp)
                                {
                                    return $@"
                        Hello,

                        You requested to verify your email address for Craftoria App.

                        Your One-Time Password (OTP) is: **{otp}**

                        This OTP is valid for 10 minutes. Please do not share it with anyone.

                        If you did not request this code, please ignore this email.

                        Thank you,
                        Craftoria Team
                        ";
                                }
            private static string GenerateOTP()=> new Random().Next(100000, 999999).ToString();
            private async Task SendOtpEmailAsync(string email, string otp)
        {
            var body = BuildOtpEmailBody(otp);
            await _emailService.SendEmailAsync(email, "Email Verification OTP", body);
        }
            public string CreatingUserName(string email)=> email.Split('@')[0].ToLower().Trim();

        public async Task<ReturnUserDTO> GoogleLoginAsync(GoogleLoginDto googleLoginDto)
        {
            // 1. التجهيز: بنجيب كود التحقق بتاعنا من ملف الإعدادات
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { _configuration["GoogleAuth:ClientId"] }
            };

            // 2. التحقق: بنكلم جوجل نتأكد إن التوكن سليم
            // payload: دي النتيجة اللي راجعة من جوجل فيها (Email, Name, etc..)
            var payload = await GoogleJsonWebSignature.ValidateAsync(googleLoginDto.IdToken, settings);

            // 3. البحث: بنشوف هل اليوزر ده عندنا ولا لأ؟
            var user = await _userManager.FindByEmailAsync(payload.Email);

            // 4. القرار:
            if (user == null)
            {
                // === الحالة أ: المستخدم ده جديد (مش موجود في الداتابيز) ===
                // يبقى لازم نعمله "تسجيل" Register أوتوماتيك دلوقتي

                // (استخدمت دالتك عشان نعمل يوزر نيم بنفس طريقتك)
                var generatedUserName = CreatingUserName(payload.Email);

                user = new ApplicationUser
                {

                    Email = payload.Email,
                    UserName = generatedUserName, // ahmed.ali
                    DisplayName = payload.Name,   // Ahmed Ali
                    FirstName = payload.GivenName,
                    SecondName = payload.FamilyName,
                    Gender = Gender.Male,
                    EmailConfirmed = true, // جوجل أكد الإيميل خلاص، مش محتاجين نبعتله OTP
                                           // باقي البيانات ممكن تسيبيها فاضية أو بـ Default

                };


                var createResult = await _userManager.CreateAsync(user);

                if (!createResult.Succeeded)
                {
                    throw new Exception("حصل مشكلة واحنا بنسجل اليوزر الجديد جاي من جوجل");
                }

                // بنديله Role افتراضي (مثلاً User)
                string role = googleLoginDto.Role.ToString();
                await _userManager.AddToRoleAsync(user, role);
            }

            // === الحالة ب: المستخدم موجود أصلاً (أو لسه عاملينه فوق) ===
            // الخطوة النهائية: نطلعله التوكن بتاع Craftoria عشان يدخل يكمل شغل

            // (استخدمت دالتك CreateTokenAsync عشان التوكن يبقى واحد في كل السيستم)
            var jwtToken = await CreateTokenAsync(user);

            // 5. الرد: بنرجع البيانات في الشكل اللي أنتي عاملاه (ReturnUserDTO)
            return new ReturnUserDTO
            {
                Email = user.Email,
                UserName = user.DisplayName, // أو user.UserName حسب ما تحبي تعرضي إيه
                Token = jwtToken
            };
        }

        public async Task ExpertWithGoolgeService(ExpertWithGoolgeDto infos)
        {
            var user = await _userManager.FindByEmailAsync(infos.Email);
            if (user == null)
            {
                throw new UserNotFoundException(infos.Email);

            }
            var portfolioPath =await  _cloudinary.UploadAsync(infos.portfolio);
            
               user.Portfolio= portfolioPath;
            user.YearsOfExperience = infos.YearsOfExp;


            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        }
    }

}
