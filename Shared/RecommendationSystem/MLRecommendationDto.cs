using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shared.RecommendationSystem
{
    public class MLRecommendationDto
    {
        [JsonPropertyName("recommendations")] 
        public List<RecommendationItemDto> RecommendationsItems { get; set; }
    }
}
