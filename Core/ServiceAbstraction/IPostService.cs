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
        Task<IEnumerable<PostResponseDto>> GetAllPostsAsync(int pageNumber, int pageSize, string userId);        // عمل لايك أو إلغاؤه
        Task<bool> ToggleLikeAsync(string userId, int postId);
        // إضافة كومنت
        Task<CommentResponseDto> AddCommentAsync(string userId, int postId, string text);
        Task<IEnumerable<CommentResponseDto>> GetPostCommentsAsync(int postId, int pageNumber, int pageSize);
        Task<PostResponseDto> GetPostByIdAsync(int postId, string userId = null);
        Task<int> GetUserPostsCountAsync(string userId); // لجلب عدد البوستات
        Task<bool> DeletePostAsync(int postId, string userId); // لمسح البوست
        Task<IEnumerable<PostResponseDto>> GetUserPostsAsync(string profileUserId, string currentUserId, int pageNumber, int pageSize);
    }
}
