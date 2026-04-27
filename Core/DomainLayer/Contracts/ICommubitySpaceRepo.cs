using DomainLayer.Models.Community_Space;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Contracts
{
    public interface ICommubitySpaceRepo
    {
        Task AddPostAsync(Post post);
        Task<Post> GetPostByIdAsync(int postId);
        Task<IEnumerable<Post>> GetAllPostsAsync();
        Task UpdatePostAsync(Post post);
        Task DeletePostAsync(int postId);
        Task AddCommentAsync(Comment comment);
        Task<Comment> GetCommentByIdAsync(int commentId);
        Task<IEnumerable<Comment>> GetRepliesOnCommentByIdAsync(int commentId);
        Task<IEnumerable<Comment>> GetAllCommentsAsync(int PosrId );
        Task UpdateCommentAsync(Comment comment);
        Task DeleteCommentAsync(int commentId);
        Task ToggleLoveAsync(PostLoves love);
        Task<int> GetCountOfLoves(int postId);
        Task<int> GetCountOfCommets(int postId);


    }
}
