using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.Identity;
using DomainLayer.Models.Interaction;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceAbstraction;
using Shared.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AccountService(UserManager<ApplicationUser> _userManager,IUnitOfWork _unitOfWork) : IAccountService
    {
        public async Task<ReturnAccountDto> GetAccount(string UserId)
        {
            var user =await _userManager.Users.FirstOrDefaultAsync(u => u.Id == UserId);
            if(user == null)
            {
                throw new UserNotFoundException("User not found");
            }
            var role =await _userManager.GetRolesAsync(user);
            var role1 = role.FirstOrDefault();
            return new ReturnAccountDto
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                SecondName = user.SecondName,
                PicturUrl = user.ProfileImage!,
                Bio = user.Bio??"",
                Specialization = user.Specialization?? "",
                Role = role1!,
                YearOfExperience = user.YearsOfExperience ?? 0
            };

        }

        public async Task<IEnumerable<ReviewsBelongToUSer>> GetReviewOfCustomer(string CustomerId)
        {

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == CustomerId);
            var reviews = await _unitOfWork.GetRepository<UserInteraction, int>().GetAllQueryable()
                .Where(r => r.UserId == CustomerId)
               .Select(r => new ReviewsBelongToUSer
               {
                   ItemId = r.ProductId ?? r.RawMaterialId ?? 0,
                   UserId = r.UserId,
                   review = r.Review ?? "",
                   Rating = r.Rating,
                   PicturUrl = r.Product != null ? r.Product.ImageUrl : (r.RawMaterial != null ? r.RawMaterial.ImageUrl : ""),
                   FirstName = user.FirstName,
                   SecondName = user.SecondName,
                   CreatedAt = r.CreatedAt
               }).ToListAsync();
            return reviews;
        }
    }
}
