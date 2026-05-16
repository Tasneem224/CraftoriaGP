using DomainLayer.Contracts;
using DomainLayer.Models.Categories;
using DomainLayer.Models.Favourite;
using ServiceAbstraction;
using Shared.Favourites;
using Shared.IdentityModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service
{
    public class FavouriteService : IFavouriteService
    {
        private readonly IUnitOfWork _unitOfWork;

        // 💡 تم تنظيف الـ Constructor وحذف HttpContextAccessor
        public FavouriteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // 🔹 أصبحت الميثود تستقبل userId مباشرة
        public async Task<string> ToggleFavouriteProductAsync(string userId, int productId)
        {
            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");

            var existingFav = await _unitOfWork.Favourites.GetFavouriteProductAsync(userId, productId);

            if (existingFav != null)
            {
                _unitOfWork.Favourites.Remove(existingFav);
                await _unitOfWork.SaveChangesAsync();
                return isArabic ? "تم الحذف من المفضلة" : "Removed from favourites";
            }
            else
            {
                var newFav = new Favourite
                {
                    UserId = userId,
                    ProductId = productId
                };

                await _unitOfWork.Favourites.AddAsync(newFav);
                await _unitOfWork.SaveChangesAsync();
                return isArabic ? "تمت الاضافة الى المفضلة" : "Added to favourites";
            }
        }

        // 🔹 أصبحت الميثود تستقبل userId مباشرة
        public async Task<string> ToggleFavouriteMaterialAsync(string userId, int materialId)
        {
            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");

            var existingFav = await _unitOfWork.Favourites.GetFavouriteMaterialAsync(userId, materialId);

            if (existingFav != null)
            {
                _unitOfWork.Favourites.Remove(existingFav);
                await _unitOfWork.SaveChangesAsync();
                return isArabic ? "تم الحذف من المفضلة" : "Removed from favourites";
            }
            else
            {
                var newFav = new Favourite
                {
                    UserId = userId,
                    RawMaterialId = materialId
                };

                await _unitOfWork.Favourites.AddAsync(newFav);
                await _unitOfWork.SaveChangesAsync();
                return isArabic ? "تمت الاضافة الى المفضلة" : "Added to favourites";
            }
        }

        // 🔹 أصبحت الميثود تستقبل userId مباشرة
        public async Task<List<FavouriteItemDto>> GetUserFavouritesAsync(string userId)
        {
            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");

            var favs = await _unitOfWork.Favourites.GetFavouritesByUserIdAsync(userId);

            return favs.Select(f => new FavouriteItemDto
            {
                Id = (f.ProductId != null && f.ProductId != 0) ? f.ProductId.Value : (f.RawMaterialId ?? 0),
                Name = isArabic ? (f.Product?.NameAr ?? f.RawMaterial?.NameAr) : (f.Product?.NameEn ?? f.RawMaterial?.NameEn),
                ImageUrl = f.Product?.ImageUrl ?? f.RawMaterial?.ImageUrl,

                // 🛠️ تم إصلاح علامة الاستفهام هنا (?.) لمنع الـ NullReferenceException الكارثي
                category = isArabic
                    ? (f.Product?.Category?.NameAr ?? f.RawMaterial?.Category?.NameAr)
                    : (f.RawMaterial?.Category?.NameEn ?? f.Product?.Category?.NameEn),

                Price = (f.Product != null && f.Product?.Price != 0)
                    ? f.Product?.Price
                    : (f.RawMaterial?.Price ?? 0)
            }).ToList();
        }
    }
}