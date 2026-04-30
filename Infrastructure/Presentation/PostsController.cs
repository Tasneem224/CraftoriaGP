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

        [HttpPost("{id}/comments")]
        public async Task<IActionResult> AddComment(int id, [FromBody] string text)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _postService.AddCommentAsync(userId, id, text);
            return Ok(result);
        }
    }
}
