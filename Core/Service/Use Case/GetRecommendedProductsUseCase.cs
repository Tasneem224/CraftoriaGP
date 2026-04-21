using DomainLayer.Contracts;
using DomainLayer.Models.Items;
using Microsoft.EntityFrameworkCore;
using ServiceAbstraction;
using Shared.RecommendationSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Use_Case
{
    public class GetRecommendedProductsUseCase : IGetRecommendedProductsUseCase
    {
        private readonly IRecommendationService _mlService; // Interface للمـ ML
        private readonly IUserInteractionRepository _userInteractionRepository; 
        private readonly IUnitOfWork _unitOfWork; 


        public GetRecommendedProductsUseCase(
            IRecommendationService mlService,
            IUserInteractionRepository userInteractionRepository,
            IUnitOfWork unitOfWork)
        {
            _mlService = mlService;
            _userInteractionRepository = userInteractionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<returnProductRecommendedDto>> ExecuteAsync(int productId)
        {
            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");

            // 1. نادي الـ ML عشان تاخدي الـ IDs والـ Scores بس
            var mlResponse = await _mlService.GetRecommendedProductIdsAsync(productId, 10);

            // لو الـ ML رجع null أو لستة فاضية (زي حالة الـ 404 للمنتجات الجديدة)
            if (mlResponse == null || !mlResponse.Any()) return new List<returnProductRecommendedDto>();

            // استخراج الـ IDs من الـ DTO (تعديل الـ Syntax هنا)
            var recommendedIds = mlResponse.Select(r => r.ProductId).ToList();

            // 2. روحي للداتابيز هاتي البيانات "الطازة"
            var productRepo = _unitOfWork.GetRepository<Product, int>();

            var dbProducts = await productRepo.GetAllQueryable()
                .Include(p => p.Interactions) // مهم جداً عشان التقييمات
                .Where(p => recommendedIds.Contains(p.Id)) // تعديل الـ Syntax هنا
                .ToListAsync();

            // 3. الربط (Mapping) مع الحفاظ على ترتيب الـ ML
            return recommendedIds
                .Select(id => {
                    var p = dbProducts.FirstOrDefault(x => x.Id == id);

                    // لو المنتج موجود في الـ ML بس مش موجود في الداتابيز (تم مسحه مثلاً)
                    if (p == null) return null;

                    return new returnProductRecommendedDto
                    {
                        Id = p.Id,
                        Name = isArabic ? p.NameAr : p.NameEn,
                        Price = p.Price, // ضمان إن السعر حقيقي
                        ImageUrl = p.ImageUrl,
                        // حساب التقييم لحظياً من الـ Interactions
                        AverageRating = p.Interactions != null && p.Interactions.Any(i => i.Rating > 0)
                                        ? p.Interactions.Average(i => i.Rating) : 0
                    };
                })
                .Where(dto => dto != null) // شيل أي منتجات null
                .Cast<returnProductRecommendedDto>() // للتأكد من الـ Type
                .ToList();
        }
    }
}
