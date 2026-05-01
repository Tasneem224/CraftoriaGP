using DomainLayer.Models.CommunitySpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Contracts
{
    public interface IPostRepository : IGenericRepository<Post, int>
    {
        Task<IEnumerable<Post>> GetPostsWithDataAsync(int pageNumber, int pageSize);
        Task<PostLike?> GetLikeAsync(string userId, int postId);
        Task AddLikeAsync(PostLike like);
        void RemoveLike(PostLike like);
    }
}
