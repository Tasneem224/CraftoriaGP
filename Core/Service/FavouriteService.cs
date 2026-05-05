using DomainLayer.Contracts;
using DomainLayer.Models.Categories;
using DomainLayer.Models.Favourite;
using Microsoft.AspNetCore.Http;
using ServiceAbstraction;
using Shared.Favourites;
using Shared.IdentityModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class FavouriteService : IFavouriteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FavouriteService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<string> ToggleFavouriteProductAsync(int productId)
        {
            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");

            var userId = AuthFun(isArabic);
            var existingFav = await _unitOfWork.Favourites.GetFavouriteProductAsync(userId, productId);

            if (existingFav != null)
            {
                _unitOfWork.Favourites.Remove(existingFav);
                await _unitOfWork.SaveChangesAsync();
                return "Removed from favourites";
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
                return isArabic? "تمت الاضافة الى المفضلة": "Added to favourites";
            }
        }
        public async Task<string> ToggleFavouriteMaterialAsync(int materialId)
        {
            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");

            var userId = AuthFun(isArabic);
            var existingFav = await _unitOfWork.Favourites.GetFavouriteMaterialAsync(userId, materialId);

            if (existingFav != null)
            {
                _unitOfWork.Favourites.Remove(existingFav);
                await _unitOfWork.SaveChangesAsync();
                return "Removed from favourites";
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
                return isArabic? "تمت الاضافة الى المفضلة": "Added to favourites";
            }
        }
        public async Task<List<FavouriteItemDto>> GetUserFavouritesAsync()
        {
            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");
            var sellerId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(sellerId))
                throw new UnauthorizedAccessException(isArabic ? "يجب تسجيل الدخول أولاً" : "Unauthorized: Please login");

            var userId = AuthFun(isArabic);
           
            var favs = await _unitOfWork.Favourites.GetFavouritesByUserIdAsync(userId);
            return favs.
                Select(f => new FavouriteItemDto{
                    Id = (f.ProductId != null && f.ProductId != 0) ? f.ProductId.Value : (f.RawMaterialId ?? 0),
                    Name =isArabic?( f.Product?.NameAr?? f.RawMaterial?.NameAr) : (f.Product?.NameEn ??f.RawMaterial?.NameEn),
                    ImageUrl = f.Product?.ImageUrl ?? f.RawMaterial?.ImageUrl,
                    category=isArabic?( f.Product?.Category?.NameAr ?? f.RawMaterial?.Category?.NameAr):(f.RawMaterial?.Category?.NameEn??f.Product?.Category.NameEn),
                    Price = (f.Product != null && f.Product?.Price != 0)
                    ? f.Product?.Price
                    : (f.RawMaterial?.Price ?? 0)
                    }).ToList();
        }
        private string? AuthFun(bool isArabic)
        {
            var sellerId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(sellerId))
                throw new UnauthorizedAccessException(isArabic ? "يجب تسجيل الدخول أولاً" : "Unauthorized: Please login");

            var user = _httpContextAccessor?.HttpContext?.User;


            if (user==null)
            {
                throw new InvalidOperationException(isArabic
                    ? "يجب عليك تسجيل الدخول"
                    : "you must be Authoruze");
            }
            return sellerId;
        }
    }
}
