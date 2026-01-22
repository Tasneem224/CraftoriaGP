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
        // تجيب OTP صالح لنفس Email
        Task<EmailVerificationCodes?> GetActiveOtpByEmailAsync(string email);

        // تعيّن كل OTPs القديمة لنفس Email كـ IsOld=true
        Task MarkOldOtpsAsync(string email);

        // توليد OTP جديد وحفظه
        Task CreateOtpAsync(string email, string otp, DateTime expirationTime);

        // تحقق من OTP المدخل
        Task<bool> VerifyOtpAsync(string email, string otp);

        // Job دوري لمسح OTPs منتهية الصلاحية (اختياري)
        Task CleanupExpiredOtpsAsync();
    }
}
