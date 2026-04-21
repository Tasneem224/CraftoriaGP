using Microsoft.AspNetCore.Mvc;
using Service.Use_Case;
using ServiceAbstraction;
using Shared.RecommendationSystem;

namespace Presentation.Controllers
{
    public class RecommendationsController : BaseApiController
    {
        private readonly IGetRecommendedProductsUseCase _getRecommendedProductsUseCase;

        public RecommendationsController(IGetRecommendedProductsUseCase getRecommendedProductsUseCase)
        {
            _getRecommendedProductsUseCase = getRecommendedProductsUseCase;
        }

        /// <summary>
        /// الحصول على منتجات مقترحة بناءً على منتج معين (Collaborative Filtering)
        /// </summary>
        /// <param name="productId">ID المنتج الحالي</param>
        
        [HttpGet("collaborative/{productId}")]
        public async Task<ActionResult<List<returnProductRecommendedDto>>> GetCollaborativeRecommendations(int productId)
        => SendSuccessResponse(await _getRecommendedProductsUseCase.ExecuteAsync(productId));
        
    }
}