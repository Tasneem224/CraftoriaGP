using Shared.RecommendationSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Use_Case
{
    public interface IGetRecommendedProductsUseCase
    {
        Task<List<returnProductRecommendedDto>> ExecuteAsync(int productId);
    }
}
