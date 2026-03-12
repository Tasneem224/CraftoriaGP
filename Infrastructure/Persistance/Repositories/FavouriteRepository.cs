using DomainLayer.Contracts;
using DomainLayer.Models.Favourite;
using Microsoft.EntityFrameworkCore;
using Persistance.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class FavouriteRepository : GenericRepository<Favourite, int>, IFavouriteRepository
    {
        private readonly StoreDbContext _context;

        public FavouriteRepository(StoreDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Favourite>> GetFavouritesByUserIdAsync(string userId)
        {
            return await _context.Favourites
               .Where(f => f.UserId == userId)
        .Include(f => f.Product)
            .ThenInclude(p => p.Category) // ده السطر السحري اللي هيمنع الـ NullReference
        .ToListAsync();
        }
        public async Task<Favourite?> GetFavouriteAsync(string userId, int productId)
        {
            return await _context.Favourites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);
        }
    }
}
