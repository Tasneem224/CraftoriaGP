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
        =>
            
             SendSuccessResponse(await _postService.GetAllPostsAsync(pageNumber, pageSize), "Posts retrieved successfully");
        

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

        // 4. إنشاء بوست جديد
        [HttpPost]
        public async Task<ActionResult<ApiResponse<IEnumerable<PostResponseDto>>>> CreatePost([FromForm] PostCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // جلب الـ ID بتاع اليوزر اللي عامل Login

            var result = await _postService.CreatePostAsync(userId, dto);
            return SendSuccessResponse(result, "Post is created successfully");
        }

        // 5. جلب كومنتات البوست
        [HttpGet("{id}/comments")]
        public async Task<ActionResult<ApiResponse<IEnumerable<CommentResponseDto>>>> GetPostComments(int id)
        {
            var result = await _postService.GetPostCommentsAsync(id);
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
    }
}