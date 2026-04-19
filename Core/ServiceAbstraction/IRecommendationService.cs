using Shared.RecommendationSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IRecommendationService
    {
        Task<List<RecommendationItemDto>> GetPopularProductIdsAsync(int topN);
        Task<List<RecommendationItemDto>> GetRecommendedProductIdsAsync(int productId, int topN);
    }
}
