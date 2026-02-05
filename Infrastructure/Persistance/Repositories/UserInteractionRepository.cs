using DomainLayer.Contracts;
using DomainLayer.Models.Interaction;
using Microsoft.EntityFrameworkCore;
using Persistance.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    internal class UserInteractionRepository : IUserInteractionRepository
    {
        private readonly StoreDbContext _DbContext;
        public UserInteractionRepository(StoreDbContext storeDbContext)
        {
            _DbContext = storeDbContext;
        }
        public async Task<int> GetReviewCountAsync(string targetId, InteractionTargetType type)
        {
            return await _DbContext.UserInteractions
                .Where(x =>
                    x.TargetId == targetId &&
                    x.TargetType == type &&
                    x.Review != null
                )
                .CountAsync();
        }

        public async Task AddAsync(UserInteraction interaction)
        {
           await _DbContext.UserInteractions.AddAsync(interaction);
        }

        public async Task<IEnumerable<UserInteraction>> GetAllForTargetAsync(string targetId, InteractionTargetType targetType)
        {
            return await _DbContext.UserInteractions.
                                    Where(x =>
                                    x.TargetId == targetId
                                    && x.TargetType == targetType
                                    && x.Review != null
                                    ).ToListAsync();
        }

        public async Task<UserInteraction?> GetAsync(string userId, string targetId, InteractionTargetType targetType)
        {
            return await _DbContext.UserInteractions.
                 FirstOrDefaultAsync(x =>
                 x.UserId == userId &&
                 x.TargetId == targetId &&
                 x.TargetType == targetType&&
                 x.Review != null

         
                );
        }

        public void Remove(UserInteraction interaction)
        {

            _DbContext.UserInteractions.Remove(interaction);
        }

        public void Update(UserInteraction interaction)
        {

            _DbContext.UserInteractions.Update(interaction);
        }

        public async Task<double> GetAverageRatingAsync(string targetId, InteractionTargetType type)
        {
            var query = _DbContext.UserInteractions
                .Where(x =>
                    x.TargetId == targetId &&
                    x.TargetType == type &&
                    x.Rating.HasValue
                );

            return await query.AnyAsync()
                ? await query.AverageAsync(x => (double)x.Rating!)
                : 0;
        }

    }
}
