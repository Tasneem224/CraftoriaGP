using DomainLayer.Contracts;
using DomainLayer.Models.Community_Space;
using Microsoft.EntityFrameworkCore;
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
        public async Task<Comment> AddCommentAsync(Comment comment)=>
        ( await  _context.Comments.AddAsync(comment)).Entity;
          

        public async Task<Post> AddPostAsync(Post post)=>
           (await _context.Posts.AddAsync(post)).Entity;
        

        public async Task<bool> DeleteCommentAsync(int commentId)
        {
            var exist =await _context.Comments.FindAsync(commentId);
            if (exist == null) return false;
            _context.Comments.Remove(exist);
            return true;
        }

        public async Task<bool> DeletePostAsync(int postId)
        {
            var exist =await _context.Posts.FindAsync(postId);
            if (exist == null) return false;
            _context.Posts.Remove(exist);
            return true;
        }

        public  async Task<IEnumerable<Comment>> GetAllCommentsAsync(int PosrId)=>
            await _context.Comments.Where(c => c.PostId == PosrId).ToListAsync();
            
        

        public async Task<IEnumerable<Post>> GetAllPostsAsync()=>
            await _context.Posts.ToListAsync();

        public Task<Comment> GetCommentByIdAsync(int commentId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetCountOfCommets(int postId)
        {
          var exist=  await _context.Posts.FindAsync(postId);
            if(exist is null) return 0;
            return await _context.Comments.CountAsync(c => c.PostId == postId);
        }

        public async Task<int> GetCountOfLoves(int postId)
        {
            var exist = await _context.Posts.FindAsync(postId);
            if (exist is null) return 0;
            return await _context.postLoves.CountAsync(l => l.PostId == postId);
        }

        public Task<Post> GetPostByIdAsync(int postId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Comment>> GetRepliesOnCommentByIdAsync(int commentId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ToggleLoveAsync(PostLoves love)
        {
            var existingLove = await _context.postLoves
                .FirstOrDefaultAsync(l => l.PostId == love.PostId && l.UserId == love.UserId);

            if (existingLove != null)
            {
                _context.postLoves.Remove(existingLove);
                return false; 
            }
            else
            {
                await _context.postLoves.AddAsync(love);
                return true;
            }
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
