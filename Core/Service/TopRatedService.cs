using DomainLayer.Contracts;
using DomainLayer.Models.Identity;
using DomainLayer.Models.Items;
using DomainLayer.Models.RawMaterials;
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
                var product = await productRepo.GetByIdAsync(stat.ProductId.Value);

                if (product != null)
                {
                    result.Add(new TopProductsDto
                    {
                        Id = product.Id,
                        Name = product.NameEn,
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

        public async Task<List<TopSellersDto>> GetTopSellersByRoleAsync(string role, int count = 5)
        {
            // 1. نجيب كل اليوزرز اللي في الـ Role ده (Expert, Supplier, الخ)
            var usersInRole = await _userManager.GetUsersInRoleAsync(role);
            var roleUserIds = usersInRole.Select(u => u.Id).ToList();

            // لو مفيش أي يوزر في الرول ده، نرجع لستة فاضية
            if (!roleUserIds.Any()) return new List<TopSellersDto>();

            // 2. نبعت الأيديهات للـ Repository الجديد اللي لسه عاملينه
            var stats = await _unitOfWork.UserInteractions.GetTopSellerStatsByRoleUserIdsAsync(count, roleUserIds);
            var result = new List<TopSellersDto>();

            foreach (var stat in stats)
            {
                // 3. نجيب بيانات اليوزر الحقيقية
                var user = await _userManager.FindByIdAsync(stat.SellerId);

                if (user != null)
                {
                    result.Add(new TopSellersDto
                    {
                        SellerId = user.Id,
                        Name = user.DisplayName,
                        ImageUrl = user.ProfileImage,
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

        public async Task<List<TopRawMaterialsDto>> GetTopRawMaterialsAsync(int count = 5)
        {
            // 1. نجيب الإحصائيات الخاصة بالمواد الخام
            var stats = await _unitOfWork.UserInteractions.GetTopRawMaterialStatsAsync(count);

            var result = new List<TopRawMaterialsDto>();

            // 2. نجيب Repository المواد الخام
            var materialRepo = _unitOfWork.GetRepository<RawMaterial, int>();

            foreach (var stat in stats)
            {
                // نتأكد إن الـ ID مش بـ null قبل ما ندور
                if (stat.RawMaterialId.HasValue)
                {
                    var material = await materialRepo.GetByIdAsync(stat.RawMaterialId.Value);

                    if (material != null)
                    {
                        result.Add(new TopRawMaterialsDto
                        {
                            Id = material.Id,
                            Name = material.NameEn,
                            ImageUrl = material.ImageUrl,
                            Price = material.Price,
                            AverageRating = Math.Round(stat.AverageRating, 1),
                            ReviewCount = stat.ReviewCount
                        });
                    }
                }
            }
            return result;
        }


    }

}
