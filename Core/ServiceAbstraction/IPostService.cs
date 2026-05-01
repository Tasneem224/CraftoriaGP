using Shared.CommunityModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IPostService
    {
        // إنشاء بوست
        Task<PostResponseDto> CreatePostAsync(string userId, PostCreateDto dto);
        // عرض كل البوستات (Pagination)
        Task<IEnumerable<PostResponseDto>> GetAllPostsAsync(int pageNumber, int pageSize);
        // عمل لايك أو إلغاؤه
        Task<bool> ToggleLikeAsync(string userId, int postId);
        // إضافة كومنت
        Task<CommentResponseDto> AddCommentAsync(string userId, int postId, string text);
        Task<IEnumerable<CommentResponseDto>> GetPostCommentsAsync(int postId);
        Task<PostResponseDto> GetPostByIdAsync(int postId, string userId = null);
    }
}
