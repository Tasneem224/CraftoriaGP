using Shared.Favourites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IFavouriteService
    {
        Task<string> ToggleFavouriteProductAsync(string userId, int productId);
        Task<string> ToggleFavouriteMaterialAsync(string userId, int materialId);
        Task<List<FavouriteItemDto>> GetUserFavouritesAsync(string userId);
    }
}
