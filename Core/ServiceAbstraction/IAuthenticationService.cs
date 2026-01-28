using Shared.IdentityModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IAuthenticationService
    {
        Task<ReturnUserDTO> RegisterAsync(RegisterDto RegisterDto);
        Task<ReturnUserDTO> LoginAsync(LoginDTO loginDto);
        Task<string> ForgotPasswordAsync(string email);
        Task<bool> VerifyOtpAsync(VerifyOtpDto model);
        Task<string> ResetPasswordAsync(ResetPasswordDto model);
        Task<ReturnEmailOTP> VerifyEmailAsync(VerifyEmailDTO email);
        Task<bool> CheckEmailOTPAsync(VerifyOtpDto emailOTP);

    }
}
