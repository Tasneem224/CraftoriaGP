using DomainLayer.Contracts;
using DomainLayer.Models.Interaction;
using Google;
using Microsoft.EntityFrameworkCore;
using Persistance.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class UserInteractionRepository : GenericRepository<UserInteraction, int>, IUserInteractionRepository
    {
        private readonly StoreDbContext _context;

        public UserInteractionRepository(StoreDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<UserInteraction?> GetByProductAsync(string userId, int productId)
                   => await _context.UserInteractions.FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);

        public async Task<UserInteraction?> GetByRawMaterialAsync(string userId, int rawMaterialId)
             => await _context.UserInteractions.FirstOrDefaultAsync(x => x.UserId == userId && x.RawMaterialId == rawMaterialId);

        public async Task<UserInteraction?> GetByTargetUserAsync(string userId, string targetUserId)
             => await _context.UserInteractions.FirstOrDefaultAsync(x => x.UserId == userId && x.TargetUserId == targetUserId);

        public async Task<IEnumerable<UserInteraction>> GetReviewsByProductIdAsync(int productId)
        {
            return await _context.UserInteractions
                .Where(x => x.ProductId == productId)
                .Include(x => x.User)
                .OrderByDescending(x => x.InteractionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserInteraction>> GetReviewsByRawMaterialIdAsync(int rawMaterialId)
        {
            return await _context.UserInteractions
                            .Where(x => x.RawMaterialId == rawMaterialId)
                            .Include(x => x.User)
                            .OrderByDescending(x => x.InteractionDate)
                            .ToListAsync();
        }

        public async Task<IEnumerable<UserInteraction>> GetReviewsByTargetUserIdAsync(string targetUserId)
        {
            return await _context.UserInteractions
                .Where(x => x.TargetUserId == targetUserId)
                .Include(x => x.User)
                .OrderByDescending(x => x.InteractionDate)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountByProductIdAsync(int productId)
        {
            return await _context.UserInteractions
                .CountAsync(x => x.ProductId == productId);
        }
        public async Task<double> GetAverageRatingByProductIdAsync(int productId)
        {
            var query = _context.UserInteractions.Where(x => x.ProductId == productId);

            if (!await query.AnyAsync()) return 0.0;

            return await query.AverageAsync(x => (double)x.Rating);

        }

        public async Task<int> GetTotalCountByRawMaterialIdAsync(int rawMaterialId)
        {
            return await _context.UserInteractions
                .CountAsync(x => x.RawMaterialId == rawMaterialId);
        }
        public async Task<double> GetAverageRatingByRawMaterialIdAsync(int rawMaterialId)
        {
            var query = _context.UserInteractions.Where(x => x.RawMaterialId == rawMaterialId);

            if (!await query.AnyAsync()) return 0.0;

            return await query.AverageAsync(x => (double)x.Rating);
        }

        public async Task<int> GetTotalInteractionsCountForUserAsync(string userId)
        {
            return await _context.UserInteractions.CountAsync(x =>
                x.TargetUserId == userId ||
                (x.Product != null && x.Product.SellerId == userId) ||
                (x.RawMaterial != null && x.RawMaterial.supplierId == userId));
        }
        public async Task<double> GetAverageRatingForUserAsync(string userId)
        {
            var query = _context.UserInteractions.Where(x =>
                x.TargetUserId == userId ||
                (x.Product != null && x.Product.SellerId == userId) ||
                (x.RawMaterial != null && x.RawMaterial.supplierId == userId));

            if (!await query.AnyAsync()) return 0.0;
            return await query.AverageAsync(x => (double)x.Rating);
        }

    }
}
