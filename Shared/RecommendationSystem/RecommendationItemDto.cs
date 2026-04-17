using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.RecommendationSystem
{
    public class RecommendationItemDto
    {
        [JsonPropertyName("product_id")]
        public int ProductId { get; set; }

        [JsonPropertyName("correlation_score")]
        public double CorrelationScore { get; set; }
    }
}
