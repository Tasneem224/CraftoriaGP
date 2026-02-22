using DomainLayer.Contracts;
using DomainLayer.Models.Favourite;
using Microsoft.Extensions.Localization;
using ServiceAbstraction;
using Shared.Favourites;
using Shared.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Service
{
    public class FavouriteService : IFavouriteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;

        public FavouriteService(IUnitOfWork unitOfWork, IStringLocalizer<SharedResources> stringLocalizer)
        {
            _unitOfWork = unitOfWork;
            _stringLocalizer = stringLocalizer;
        }

        public async Task<string> ToggleFavouriteAsync(string userId, int productId)
        {
            // هنا كان بيضرب عشان مكانش عارف يعني إيه Favourites جوه الـ UnitOfWork
            // تأكدي إنك ضفتيها في Interface و Class الـ UnitOfWork زي ما اتفقنا
            var existingFav = await _unitOfWork.Favourites.GetFavouriteAsync(userId, productId);

            if (existingFav != null)
            {
                _unitOfWork.Favourites.Remove(existingFav);
                await _unitOfWork.SaveChanges();
                //return "Removed from favourites";
                return _stringLocalizer[SharedResourcesKeys.RemoveFav];
            }
            else
            {
                var newFav = new Favourite // << كان بيضرب هنا عشان مش لاقي الـ Namespace
                {
                    UserId = userId,
                    ProductId = productId
                };

                await _unitOfWork.Favourites.AddAsync(newFav);
                await _unitOfWork.SaveChanges();
                // return "Added to favourites";
                return _stringLocalizer[SharedResourcesKeys.AddToFav];
            }
        }

        public async Task<List<FavouriteItemDto>> GetUserFavouritesAsync(string userId)
        {
            var favs = await _unitOfWork.Favourites.GetFavouritesByUserIdAsync(userId);

            // كان بيضرب هنا عشان مش عارف DTO
            return favs.Select(f => new FavouriteItemDto
            {
                Id = f.Product.Id,
                Name = f.Product.NameEn,
                ImageUrl = f.Product.ImageUrl,
                Price = f.Product.Price
            }).ToList();
        }
    }
}
