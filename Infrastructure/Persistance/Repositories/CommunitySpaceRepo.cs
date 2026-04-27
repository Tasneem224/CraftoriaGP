using DomainLayer.Contracts;
using DomainLayer.Models.Community_Space;
using Persistance.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    internal class CommunitySpaceRepo(StoreDbContext _context) : ICommubitySpaceRepo
    {
        public Task AddCommentAsync(Comment comment)
        {
            throw new NotImplementedException();
        }

        public Task<Post> AddPostAsync(Post post)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteCommentAsync(int commentId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeletePostAsync(int postId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Comment>> GetAllCommentsAsync(int PosrId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Post>> GetAllPostsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Comment> GetCommentByIdAsync(int commentId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetCountOfCommets(int postId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetCountOfLoves(int postId)
        {
            throw new NotImplementedException();
        }

        public Task<Post> GetPostByIdAsync(int postId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Comment>> GetRepliesOnCommentByIdAsync(int commentId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ToggleLoveAsync(PostLoves love)
        {
            throw new NotImplementedException();
        }

        public Task<Post> UpdateCommentAsync(Comment comment)
        {
            throw new NotImplementedException();
        }

        public Task<Post> UpdatePostAsync(Post post)
        {
            throw new NotImplementedException();
        }
    }
}
