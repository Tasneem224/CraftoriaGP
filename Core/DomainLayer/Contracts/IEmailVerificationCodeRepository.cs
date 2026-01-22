using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Contracts
{
    public interface IEmailVerificationCodeRepository
    {
        Task<EmailVerificationCodes?> GetActiveOtpByEmailAsync(string email);

        Task MarkOldOtpsAsync(string email);

        Task CreateOtpAsync(string email, string otp, DateTime expirationTime);

        Task<bool> VerifyOtpAsync(string email, string otp);
        Task CleanupExpiredOtpsAsync();
    }
}
