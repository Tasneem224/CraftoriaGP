using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Presentation.Controllers;
using ServiceAbstraction;
using Shared.Interaction;
using Shared.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    public class ReviewsController:BaseApiController
    {
        private readonly IUserInteractionService _reviewService;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;


        public ReviewsController(IUserInteractionService reviewService, IStringLocalizer<SharedResources> stringLocalizer)
        {
            _reviewService = reviewService;
            _stringLocalizer = stringLocalizer;
        }

        [Authorize]
        [HttpPost("AddOrUpdateReview")]
        public async Task<IActionResult> AddOrUpdateReview( AddReviewDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            if (dto.ProductId == null && dto.RawMaterialId == null && dto.TargetUserId == null)
                return BadRequest(_stringLocalizer[SharedResourcesKeys.DetermineAnItemFirst]);

           var review= await _reviewService.AddOrUpdateReviewAsync(userId, dto);

            return SendSuccessResponse(review);
        }

        [Authorize]
        [HttpDelete("DeleteReview")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return Unauthorized(_stringLocalizer[SharedResourcesKeys.UserIdNotFoundInToken]);

             await _reviewService.DeleteReviewAsync(reviewId, userId);
                return SendSuccessResponse(_stringLocalizer[SharedResourcesKeys.ReviewDeletedSuccessfully]);
        }

        [HttpGet("GetProductReviews")]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var reviews = await _reviewService.GetProductReviewsAsync(productId);
            return SendSuccessResponse(reviews);
        }
        [HttpGet("GetRawMaterialReviews")]
        public async Task<IActionResult> GetRawMaterialReviews(int materialId)
        {
            var reviews = await _reviewService.GetRawMaterialReviewsAsync(materialId);
            return SendSuccessResponse(reviews);
        }
        [HttpGet("GetUserReviews")]
        public async Task<IActionResult> GetUserReviews(string targetUserId)
        {
            var reviews = await _reviewService.GetUserReviewsAsync(targetUserId);
            return SendSuccessResponse(reviews);
        }

        [HttpGet("GetProductStats")]
        public async Task<IActionResult> GetProductStats(int productId)
        {
            var stats = await _reviewService.GetProductStatsAsync(productId);
            return SendSuccessResponse(stats);
        }

        [HttpGet("GetRawMaterialStats")]
        public async Task<IActionResult> GetRawMaterialStats(int materialId)
        {
            var stats = await _reviewService.GetRawMaterialStatsAsync(materialId);
            return SendSuccessResponse(stats);
        }
        [HttpGet("GetUserStats")]
        public async Task<IActionResult> GetUserStats(string targetUserId)
        {
            var stats = await _reviewService.GetUserStatsAsync(targetUserId);
            return SendSuccessResponse(stats);
        }
    }

}
