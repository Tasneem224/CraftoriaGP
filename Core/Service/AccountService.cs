using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.Identity;
using DomainLayer.Models.Interaction;
using DomainLayer.Models.Items;
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
                ProfileImage = user.ProfileImage!,
                Bio = user.Bio??"",
                Specialization = user.Specialization?? "",
                Role = role1!,
                YearOfExperience = user.YearsOfExperience ?? 0
            };

        }

        public async Task<IEnumerable<ReviewsBelongToUSer>> GetReviewOfCustomer(string sellerId)
        {
            var reviews = await _unitOfWork
                .GetRepository<UserInteraction, int>()
                .GetAllQueryable()
                .Where(r =>
                    r.TargetUserId == sellerId
                    || (r.Product != null && r.Product.SellerId == sellerId)
                    || (r.RawMaterial != null && r.RawMaterial.supplierId == sellerId)
                )
                .Select(r => new ReviewsBelongToUSer
                {
                    ItemId = r.ProductId ?? r.RawMaterialId ?? 0,

                    UserId = r.UserId,
                    FirstName = r.User.FirstName,
                    SecondName = r.User.SecondName,

                    review = r.Review ?? "",
                    Rating = r.Rating,

                    PicturUrl =
                        r.Product != null
                            ? r.Product.ImageUrl
                            : (r.RawMaterial != null
                                ? r.RawMaterial.ImageUrl
                                : ""),

                    CreatedAt = r.CreatedAt
                })
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return reviews;
        }
    }
}
