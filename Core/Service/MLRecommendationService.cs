using Shared.RecommendationSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class MLRecommendationService
    {
        private readonly HttpClient _httpClient;
        public MLRecommendationService(HttpClient httpClient) => _httpClient = httpClient;
        public async Task<List<int>> GetRecommendedProductIdsAsync(int productId, int topN)
        {
            var response = await _httpClient.PostAsJsonAsync("recommend/collaborative", new { product_id = productId, top_n = topN });
            var data = await response.Content.ReadFromJsonAsync<MLRecommendationDto>();
            return data.RecommendationsItems.Select(x => x.ProductId).ToList();
        }
    }
}
