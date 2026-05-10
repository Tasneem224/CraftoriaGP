using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers;
using ServiceAbstraction;
using Shared;
using Shared.CommunityModule;
using Shared.ProductModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    [Authorize] // لازم يكون عامل Login
    public class PostsController : BaseApiController
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService) => _postService = postService;

        // 1. جلب كل البوستات (Pagination)
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<PostResponseDto>>>> GetAllPosts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // سحب الـ ID بتاع اليوزر اللي فاتح التطبيق دلوقتي
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // تمرير الـ userId للميثود
            var posts = await _postService.GetAllPostsAsync(pageNumber, pageSize, userId);

            return SendSuccessResponse(posts, "Posts retrieved successfully");
        }

        // 2. عمل لايك أو إلغاؤه
        [HttpPost("{id}/like")]
        public async Task<ActionResult<ApiResponse<bool>>> ToggleLike(int id)
        {
            // بنجيب الـ UserId من الـ Token بتاع الشخص اللي عامل Log in
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var result = await _postService.ToggleLikeAsync(userId, id);
            return SendSuccessResponse(result, "Post like toggled successfully");
        }

        // 3. إضافة كومنت جديد
        [HttpPost("{id}/comments")]
        public async Task<ActionResult<ApiResponse<IEnumerable<CommentResponseDto>>>> AddComment(int id, [FromBody] string text)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            if (string.IsNullOrWhiteSpace(text)) return BadRequest("Comment text cannot be empty");

            var result = await _postService.AddCommentAsync(userId, id, text);
            return SendSuccessResponse(result, "Comment added successfully");
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<IEnumerable<PostResponseDto>>>> CreatePost([FromForm] PostCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // جلب الـ ID بتاع اليوزر اللي عامل Login

            var result = await _postService.CreatePostAsync(userId, dto);
            return SendSuccessResponse(result, "Post is created successfully");
        }

        [HttpGet("{id}/comments")]

        public async Task<ActionResult<ApiResponse<IEnumerable<CommentResponseDto>>>> GetPostComments(int id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _postService.GetPostCommentsAsync(id, pageNumber, pageSize);

            return SendSuccessResponse(result, "Comments retrieved successfully");
        }

        // 6. جلب تفاصيل بوست معين
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PostResponseDto>>>> GetPostById(int id)
        {
            // بنجيب الـ UserId عشان نبعته للـ Service فتعرف تحسب الـ IsLikedByMe
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _postService.GetPostByIdAsync(id, userId);

            // لو الـ Service رجعت null معناه إن البوست اتمسح أو مش موجود
            if (result == null)
                return NotFound(new { message = "Post not found" });

            return SendSuccessResponse(result, "Post details retrieved successfully");
        }

        // 7. جلب عدد بوستات يوزر معين (بتبعتي الـ UserId في الـ URL)
        [HttpGet("user/{userId}/count")]
        public async Task<ActionResult> GetUserPostsCount(string userId)
        {
            var count = await _postService.GetUserPostsCountAsync(userId);
            return SendSuccessResponse(new { postCount = count }, "User posts count retrieved successfully");
        }

        // 8. مسح بوست
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePost(int id)
        {
            // بنجيب الـ UserId من التوكن عشان نتأكد إنه بيمسح بوسته هو بس
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var deleted = await _postService.DeletePostAsync(id, userId);

            if (!deleted)
                return BadRequest(new { message = "You are not authorized to delete this post or post doesn't exist" });

            return SendSuccessResponse(new { }, "Post deleted successfully");
        }

        // 9. جلب بوستات يوزر معين للبروفايل
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<PostResponseDto>>>> GetUserPosts(string userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // اليوزر اللي "بيتفرج" حالياً عشان نعرف هو عامل لايك ولا لأ
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _postService.GetUserPostsAsync(userId, currentUserId, pageNumber, pageSize);

            return SendSuccessResponse(result, "User posts retrieved successfully");
        }
    }
}