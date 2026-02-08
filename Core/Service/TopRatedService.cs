using DomainLayer.Contracts;
using DomainLayer.Models.Identity;
using DomainLayer.Models.Items;
using Microsoft.AspNetCore.Identity;
using ServiceAbstraction;
using Shared.TopRated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class TopRatedService : ITopRatedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager; // التعديل هنا: ApplicationUser

        public TopRatedService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<List<TopProductsDto>> GetTopProductsAsync(int count = 5)
        {
            // 1. نجيب الإحصائيات (IDs + Ratings)
            var stats = await _unitOfWork.UserInteractions.GetTopProductStatsAsync(count);

            var result = new List<TopProductsDto>();

            // 2. نجيب Repository المنتجات
            // تأكدي إن كلاس المنتج اسمه Product والـ ID نوعه int
            var productRepo = _unitOfWork.GetRepository<Product, int>();

            foreach (var stat in stats)
            {
                var product = await productRepo.GetByIdAsync(stat.ProductId);

                if (product != null)
                {
                    result.Add(new TopProductsDto
                    {
                        Id = product.Id,
                        Name = product.Name,
                        ImageUrl = product.ImageUrl,
                        Price = product.Price,
                        AverageRating = Math.Round(stat.AverageRating, 1),
                        ReviewCount = stat.ReviewCount
                    });
                }
            }
            return result;
        }

        public async Task<List<TopSellersDto>> GetTopSellersAsync(int count = 5)
        {
            // 1. نجيب إحصائيات البائعين
            var stats = await _unitOfWork.UserInteractions.GetTopSellerStatsAsync(count);
            var result = new List<TopSellersDto>();

            foreach (var stat in stats)
            {
                // 2. نجيب بيانات اليوزر الحقيقية
                var user = await _userManager.FindByIdAsync(stat.SellerId);

                if (user != null)
                {
                    result.Add(new TopSellersDto
                    {
                        SellerId = user.Id,

                        // هنا ربطنا بالداتا الحقيقية من الموديل بتاعك
                        Name = user.DisplayName,

                        // الصورة (ممكن تكون null وعادي الـ DTO بيقبلها)
                        ImageUrl = user.ProfileImage,

                        // التخصص الحقيقي (لو فاضي بنكتب بداله Artisan)
                        Speciality = !string.IsNullOrEmpty(user.Specialization)
                                     ? user.Specialization
                                     : "Artisan",

                        AverageRating = Math.Round(stat.AverageRating, 1),
                        ReviewCount = stat.ReviewCount
                    });
                }
            }
            return result;
        }
    }
}
