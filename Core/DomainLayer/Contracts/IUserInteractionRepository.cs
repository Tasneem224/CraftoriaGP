using DomainLayer.Models.Interaction;
using DomainLayer.Models.TopRated;
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


        Task<IEnumerable<UserInteraction>> GetAllReviewsOfProducBytIdAsync(int productId);
        Task<IEnumerable<UserInteraction>> GetAllReviewsOfRawMaterialByIdAsync(int rawMaterialId);
        Task<IEnumerable<UserInteraction>> GetAllReviewsOfTargetUserByIdAsync(string targetUserId);

        Task<IEnumerable<UserInteraction>>GetAllReviewThatCreatedBySpecificUser(string  userId);

        Task<int> GetTotalCountByProductIdAsync(int productId);
        Task<double> GetAverageRatingByProductIdAsync(int productId);

        Task<int> GetTotalCountByRawMaterialIdAsync(int rawMaterialId);
        Task<double> GetAverageRatingByRawMaterialIdAsync(int rawMaterialId);


        Task<int> GetTotalInteractionsCountForUserAsync(string userId);
        Task<double> GetAverageRatingForUserAsync(string userId);
        // الدوال الجديدة بترجع Stat مش Dto
        Task<List<TopRatedStat>> GetTopProductStatsAsync(int count);
        Task<List<TopRatedStat>> GetTopSellerStatsAsync(int count);
        Task<List<TopRatedStat>> GetTopRawMaterialStatsAsync(int count);
        Task<List<TopRatedStat>> GetTopSellerStatsByRoleUserIdsAsync(int count, List<string> userIds);
    }
}
