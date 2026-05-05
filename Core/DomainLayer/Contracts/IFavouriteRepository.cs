using DomainLayer.Models.Favourite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Contracts
{
    public interface IFavouriteRepository : IGenericRepository<Favourite, int>
    {
        Task<List<Favourite>> GetFavouritesByUserIdAsync(string userId);
        Task<Favourite?> GetFavouriteMaterialAsync(string userId, int materialId);
        
            Task<Favourite?> GetFavouriteProductAsync(string userId, int productId);
    }
}
