using Shared.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IUserInteractionService
    {
        Task<ReviewDto> AddOrUpdateReviewAsync(string userId, AddReviewDto dto);
        Task DeleteReviewAsync(int reviewId, string userId);
        Task<List<ReviewDto>> GetProductReviewsAsync(int productId);
        Task<List<ReviewDto>> GetRawMaterialReviewsAsync(int rawMaterialId);
        Task<List<AllReviewsOfTargetUser>> GetUserReviewsAsync(string targetUserId);
        Task<ReviewStatsDto> GetUserStatsAsync(string userId);
        Task<ReviewStatsDto> GetProductStatsAsync(int productId);
        Task<ReviewStatsDto> GetRawMaterialStatsAsync(int rawMaterialId);
    }
}
