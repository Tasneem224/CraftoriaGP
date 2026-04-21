using ServiceAbstraction;
using Shared.RecommendationSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class RecommendationService : IRecommendationService
    {
        private readonly HttpClient _httpClient;

        public RecommendationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<RecommendationItemDto>> GetPopularProductIdsAsync(int topN)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("recommend/popular", new { top_n = topN });

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<MLRecommendationDto>();
                    return result?.RecommendationsItems ?? new List<RecommendationItemDto>();
                }
            }
            catch { /* Logging */ }
            return new List<RecommendationItemDto>();
        }
        public async Task<List<RecommendationItemDto>> GetRecommendedProductIdsAsync(int productId, int topN)
        {
            var response = await _httpClient.PostAsJsonAsync("recommend/collaborative", new { product_id = productId, top_n = topN });

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<MLRecommendationDto>();
                // نرجع الـ Items كاملة
                return result?.RecommendationsItems ?? new List<RecommendationItemDto>();
            }
            return new List<RecommendationItemDto>();
        }
    }

}
