using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers;
using ServiceAbstraction;
using Shared.Interaction;
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

        public ReviewsController(IUserInteractionService reviewService)
        {
            _reviewService = reviewService;
        }

        [Authorize]
        [HttpPost("AddOrUpdateReview")]
        public async Task<IActionResult> AddOrUpdateReview([FromBody] AddReviewDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("User ID not found in token");

            if (dto.ProductId == null && dto.RawMaterialId == null && dto.TargetUserId == null)
                return BadRequest("you should determine an item first");

           var review= await _reviewService.AddOrUpdateReviewAsync(userId, dto);

            return SendSuccessResponse(review);
        }

        [Authorize]
        [HttpDelete("DeleteReview")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return Unauthorized();

             await _reviewService.DeleteReviewAsync(reviewId, userId);
                return SendSuccessResponse("Review deleted successfully");
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
