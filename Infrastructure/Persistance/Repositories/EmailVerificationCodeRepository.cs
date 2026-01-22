using DomainLayer.Contracts;
using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using Persistance.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class EmailVerificationCodeRepository : IEmailVerificationCodeRepository
    {
        private readonly StoreDbContext _context;

        public EmailVerificationCodeRepository(StoreDbContext context)
        {
            _context = context;
        }

        public async Task CleanupExpiredOtpsAsync()
        {
            var expiredOtps = await _context.Set<EmailVerificationCodes>()
                          .Where(x => x.ExpirationTime <= DateTime.UtcNow)
                          .ToListAsync();

            _context.Set<EmailVerificationCodes>().RemoveRange(expiredOtps);
            await _context.SaveChangesAsync();
        }        

        public async Task CreateOtpAsync(string email, string otp, DateTime expirationTime)
        {
            var newOtp = new EmailVerificationCodes
            {
                Email = email,
                OTP = otp,
                ExpirationTime = expirationTime,
                IsUsed = false,
                IsOld = false,
                IsVerified = false,
                CreatedAt = DateTime.UtcNow
            };
            await   _context.Set<EmailVerificationCodes>().AddAsync(newOtp);
            await   _context.SaveChangesAsync();
        }

        public async Task<EmailVerificationCodes?> GetActiveOtpByEmailAsync(string email)
        {
            return await _context.EmailVerificationCodes
                .Where(x => x.Email == email && !x.IsUsed && !x.IsOld && x.ExpirationTime > DateTime.UtcNow)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();


        }

        public Task MarkOldOtpsAsync(string email)
        {
            var oldOtps = _context.EmailVerificationCodes
                .Where(x => x.Email == email && !x.IsOld);

            foreach (var otp in oldOtps)
            {
                otp.IsOld = true;
            }
            return _context.SaveChangesAsync();
        }

        public async Task<bool> VerifyOtpAsync(string email, string otp)
        {
                 var existingOtp = await _context.Set<EmailVerificationCodes>()   
                .FirstOrDefaultAsync(x => x.Email == email &&
                                          x.OTP == otp &&
                                          !x.IsUsed &&
                                          !x.IsOld &&
                                          x.ExpirationTime > DateTime.UtcNow);

            if (existingOtp == null)
                return false;

            existingOtp.IsUsed = true;
            existingOtp.IsVerified = true;

            await _context.SaveChangesAsync();
            return true;     
        
        }
    }
}
