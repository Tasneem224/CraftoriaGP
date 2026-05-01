using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers;
using ServiceAbstraction;
using Shared.CommunityModule;
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
        public async Task<IActionResult> GetAllPosts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _postService.GetAllPostsAsync(pageNumber, pageSize);
            return Ok(result);
        }

        // 2. عمل لايك أو إلغاؤه
        [HttpPost("{id}/like")]
        public async Task<IActionResult> ToggleLike(int id)
        {
            // بنجيب الـ UserId من الـ Token بتاع الشخص اللي عامل Log in
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var result = await _postService.ToggleLikeAsync(userId, id);
            return Ok(result); // هيرجع LikeResponseDto المنظم
        }



        // 3. إضافة كومنت جديد
        [HttpPost("{id}/comments")]
        public async Task<IActionResult> AddComment(int id, [FromBody] string text)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            if (string.IsNullOrWhiteSpace(text)) return BadRequest("Comment text cannot be empty");

            var result = await _postService.AddCommentAsync(userId, id, text);
            return Ok(result); // هيرجع CommentResponseDto اللي فيه بيانات اليوزر والوقت
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromForm] PostCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // جلب الـ ID بتاع اليوزر اللي عامل Login
            var result = await _postService.CreatePostAsync(userId, dto);
            return Ok(result);
        }

        [HttpPost("{id}/like")]
        public async Task<IActionResult> LikePost(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _postService.ToggleLikeAsync(userId, id);
            return Ok(new { isLiked = result });
        }

        [HttpGet("{id}/comments")]
        public async Task<IActionResult> GetPostComments(int id)
        {
            var result = await _postService.GetPostCommentsAsync(id);

            // مش محتاجين نتشيك إذا كان البوست موجود ولا لأ، لو مش موجود هيرجع ليستة فاضية وده صح
            return Ok(result);
        }
    }
}
