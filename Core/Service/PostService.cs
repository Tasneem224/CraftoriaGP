using DomainLayer.Contracts;
using DomainLayer.Models.CommunitySpace;
using DomainLayer.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceAbstraction;
using Shared.CommunityModule;
using Shared.IdentityModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinary;

        
        private readonly UserManager<ApplicationUser> _userManager;

        public PostService(IUnitOfWork unitOfWork, ICloudinaryService cloudinary, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _cloudinary = cloudinary;
            _userManager = userManager;
        }

        public async Task<PostResponseDto> CreatePostAsync(string userId, PostCreateDto dto)
        {
            // 1. رفع الصورة بأمان (لو الموبايل بعت صورة فعلاً)
            string? imageUrl = null;
            if (dto.Image != null)
            {
                var uploadResult = await _cloudinary.UploadAsync(dto.Image);
                imageUrl = uploadResult?.ToString();
            }

            // 2. جلب بيانات اليوزر عشان محتاجين اسمه في الـ Response
            var user = await _userManager.FindByIdAsync(userId);

            // 3. تكريت البوست وإعطائه الوقت الحالي
            var post = new Post
            {
                Content = dto.Content,
                ImageUrl = imageUrl,
                UserId = userId,
                CreatedAt = DateTime.UtcNow // <-- هنا حلينا مشكلة الوقت
            };

            // 4. الحفظ في الداتابيز
            await _unitOfWork.Posts.AddAsync(post);
            await _unitOfWork.SaveChangesAsync();

            // 5. إرجاع الـ DTO متكامل ومفيش فيه حاجة Null
            return new PostResponseDto
            {
                Id = post.Id,
                Content = post.Content,
                ImageUrl = post.ImageUrl,          // <-- هنا حلينا مشكلة الصورة
                UserName = user?.UserName,         // <-- هنا حلينا مشكلة الاسم
                CreatedAt = post.CreatedAt,        // <-- تمرير الوقت للـ Response
                LikesCount = 0,                    // بوست لسه متكريت فاكيد اللايكات 0
                CommentsCount = 0,                 // وأكيد الكومنتات 0
                IsLikedByMe = false                // طبيعي لسه معملتلوش لايك
            };
        }

        public async Task<bool> ToggleLikeAsync(string userId, int postId)
        {
            // 1. اسأل الـ Repository: هل فيه لايك موجود؟
            var existingLike = await _unitOfWork.Posts.GetLikeAsync(userId, postId);

            if (existingLike != null)
            {
                // 2. لو موجود، امسحه
                _unitOfWork.Posts.RemoveLike(existingLike);
                await _unitOfWork.SaveChangesAsync();
                return false; // يعني الـ Like اتشال
            }

            // 3. لو مش موجود، ضيف لايك جديد
            var newLike = new PostLike { UserId = userId, PostId = postId };
            await _unitOfWork.Posts.AddLikeAsync(newLike);

            await _unitOfWork.SaveChangesAsync();
            return true; // يعني الـ Like انضاف
        }

        public async Task<CommentResponseDto> AddCommentAsync(string userId, int postId, string text)
        {
            // 1. إنشاء الكومنت
            var comment = new Comment
            {
                Text = text,
                PostId = postId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            // 2. الحفظ عن طريق الـ UnitOfWork
            // ملحوظة: لو معندكيش ICommentRepository استخدمي الـ GetRepository بتاعك
            await _unitOfWork.GetRepository<Comment, int>().AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();

            // 3. جلب بيانات اليوزر عشان نرجع الـ DTO كامل
            // بنجيب اليوزر من الـ UserManager أو من الـ DB بس عشان الـ Name
            var user = await _userManager.FindByIdAsync(userId);

            return new CommentResponseDto
            {
                Id = comment.Id,
                Text = comment.Text,
                UserName = user.UserName,
                CreatedAt = comment.CreatedAt,
                TimeAgo = "Just now" // أو استخدمي Helper يحسب الفرق الزمني
            };
        }

        public async Task<IEnumerable<PostResponseDto>> GetAllPostsAsync(int pageNumber, int pageSize)
        {
            // 1. بنجيب البوستات من الـ Repository (بالميثود اللي عملناها عشان تجيب الداتا كاملة)
            var posts = await _unitOfWork.Posts.GetPostsWithDataAsync(pageNumber, pageSize);

            // 2. بنحول الـ Posts لـ PostResponseDto عشان نرجعها للموبايل شكلها نضيف
            var response = posts.Select(post => new PostResponseDto
            {
                Id = post.Id,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                CreatedAt = post.CreatedAt,
                UserName = post.User?.UserName ?? "Unknown User",
                LikesCount = post.Likes?.Count ?? 0,
                CommentsCount = post.Comments?.Count ?? 0,
                IsLikedByMe = false // مؤقتاً، عشان نعرف هي بـ true محتاجين نمرر الـ userId للميثود دي قدام
            }).ToList();

            return response;
        }

        public async Task<IEnumerable<CommentResponseDto>> GetPostCommentsAsync(int postId, int pageNumber, int pageSize)
        {
            // 1. هنجيب الكومنتات مع عمل Pagination
            var comments = await _unitOfWork.GetRepository<Comment, int>()
                .GetAllQueryable()
                .Where(c => c.PostId == postId)
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt) // الأحدث الأول
                .Skip((pageNumber - 1) * pageSize)   // تخطي الكومنتات اللي شفناها قبل كده
                .Take(pageSize)                      // هات العدد المطلوب بس (مثلاً 10)
                .ToListAsync();

            // 2. التحويل لـ DTO
            var response = comments.Select(c => new CommentResponseDto
            {
                Id = c.Id,
                Text = c.Text,
                UserName = c.User?.UserName ?? "Unknown User",
                CreatedAt = c.CreatedAt,
                TimeAgo = "Just now"
            }).ToList();

            return response;
        }




        public async Task<PostResponseDto> GetPostByIdAsync(int postId, string userId = null)
        {
            // 1. هنجيب البوست مع الداتا المرتبطة بيه (اليوزر، اللايكات، الكومنتات)
            var post = await _unitOfWork.GetRepository<Post, int>()
                .GetAllQueryable()
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == postId);

            // 2. لو البوست مش موجود، هنرجع null عشان الكنترولر يتصرف
            if (post == null) return null;

            // 3. تحويل البوست لـ PostResponseDto
            return new PostResponseDto
            {
                Id = post.Id,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                CreatedAt = post.CreatedAt,
                UserName = post.User?.UserName ?? "Unknown User",
                LikesCount = post.Likes?.Count ?? 0,
                CommentsCount = post.Comments?.Count ?? 0,
                // بنتشيك هل اليوزر ده موجود جوه ليستة اللايكات بتاعة البوست ولا لأ
                IsLikedByMe = userId != null && post.Likes != null && post.Likes.Any(l => l.UserId == userId)
            };
        }
    }
}
