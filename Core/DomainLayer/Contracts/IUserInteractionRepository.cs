using DomainLayer.Models.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Contracts
{
    public interface IUserInteractionRepository:IGenericRepository<UserInteraction,int>
    {
        Task<UserInteraction?> GetByProductAsync(string userId, int productId);
        Task<UserInteraction?> GetByRawMaterialAsync(string userId, int rawMaterialId);
        Task<UserInteraction?> GetByTargetUserAsync(string userId, string targetUserId);


        Task<IEnumerable<UserInteraction>> GetReviewsByProductIdAsync(int productId);
        Task<IEnumerable<UserInteraction>> GetReviewsByRawMaterialIdAsync(int rawMaterialId);
        Task<IEnumerable<UserInteraction>> GetReviewsByTargetUserIdAsync(string targetUserId);


        Task<int> GetTotalInteractionsCountForUserAsync(string userId);
        Task<double> GetAverageRatingForUserAsync(string userId);
    }
}
