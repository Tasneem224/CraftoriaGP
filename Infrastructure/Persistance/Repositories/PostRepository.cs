using DomainLayer.Contracts;
using DomainLayer.Models.CommunitySpace;
using Microsoft.EntityFrameworkCore;
using Persistance.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class PostRepository : GenericRepository<Post, int>, IPostRepository
    {
        private readonly StoreDbContext _context;
        public PostRepository(StoreDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Post>> GetPostsWithDataAsync(int pageNumber, int pageSize)
        {
            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<PostLike?> GetLikeAsync(string userId, int postId)
        {
            return await _context.Set<PostLike>()
                .FirstOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);
        }

        public async Task AddLikeAsync(PostLike like) => await _context.Set<PostLike>().AddAsync(like);

        public void RemoveLike(PostLike like) => _context.Set<PostLike>().Remove(like);
    }
}
