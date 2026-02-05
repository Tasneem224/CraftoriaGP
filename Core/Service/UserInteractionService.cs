using DomainLayer.Contracts;
using DomainLayer.Models.Interaction;
using ServiceAbstraction;
using Shared.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class UserInteractionService : IUserInteractionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserInteractionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddOrUpdateReviewAsync(string userId, AddReviewDto dto)
        {
            UserInteraction? interaction = null;

            interaction = await checkIfThereIsAnyReviewBefore(userId, dto, interaction);

            if (interaction != null)
            {
                interaction.Rating = dto.Rating;
                interaction.Review = dto.Review;
                interaction.InteractionDate = DateTime.UtcNow;

                _unitOfWork.UserInteractions.Update(interaction);
            }
            else
            {
                interaction = new UserInteraction
                {
                    UserId = userId, 
                    ProductId = dto.ProductId,
                    RawMaterialId = dto.RawMaterialId,
                    TargetUserId = dto.TargetUserId,
                    Rating = dto.Rating,
                    Review = dto.Review,
                    InteractionDate = DateTime.UtcNow
                };

                await _unitOfWork.UserInteractions.AddAsync(interaction);
            }
        }

        private async Task<UserInteraction?> checkIfThereIsAnyReviewBefore(string userId, AddReviewDto dto, UserInteraction? interaction)
        {
            if (dto.ProductId.HasValue)
            {
                interaction = await _unitOfWork.UserInteractions.GetByProductAsync(userId, dto.ProductId.Value);
            }
            else if (dto.RawMaterialId.HasValue)
            {
                interaction = await _unitOfWork.UserInteractions.GetByRawMaterialAsync(userId, dto.RawMaterialId.Value);
            }
            else if (!string.IsNullOrEmpty(dto.TargetUserId))
            {
                interaction = await _unitOfWork.UserInteractions.GetByTargetUserAsync(userId, dto.TargetUserId);
            }

            return interaction;
        }

        public async Task DeleteReviewAsync(int reviewId, string userId)
        {
            var interaction = await _unitOfWork.UserInteractions.GetByIdAsync(reviewId);

            if (interaction == null)
                throw new Exception("Review not found");

            if (interaction.UserId != userId) throw new UnauthorizedAccessException("U can't delete this Review");

            _unitOfWork.UserInteractions.Remove(interaction);
            await _unitOfWork.SaveChanges();
        }
        public async Task<List<ReviewDto>> GetProductReviewsAsync(int productId)
        {
            var reviews = await _unitOfWork.UserInteractions.GetReviewsByProductIdAsync(productId);
            return MapToDto(reviews);
        }

        public async Task<List<ReviewDto>> GetRawMaterialReviewsAsync(int rawMaterialId)
        {
            var reviews = await _unitOfWork.UserInteractions.GetReviewsByRawMaterialIdAsync(rawMaterialId);
            return MapToDto(reviews);
        }

        public async Task<List<ReviewDto>> GetUserReviewsAsync(string targetUserId)
        {
            var reviews = await _unitOfWork.UserInteractions.GetReviewsByTargetUserIdAsync(targetUserId);
            return MapToDto(reviews);
        }

        public async Task<ReviewStatsDto> GetUserStatsAsync(string userId)
        {
            var count = await _unitOfWork.UserInteractions.GetTotalInteractionsCountForUserAsync(userId);
            var average = await _unitOfWork.UserInteractions.GetAverageRatingForUserAsync(userId);

            return new ReviewStatsDto
            {
                TotalReviews = count,
                AverageRating = Math.Round(average, 1) // نقرب لرقم عشري واحد (مثلاً 4.5)
            };
        }
        private List<ReviewDto> MapToDto(IEnumerable<UserInteraction> list)
        {
            return list.Select(x => new ReviewDto
            {
                InteractionId = x.Id,
                ReviewerId = x.UserId,
                // بنجيب الاسم من الـ Navigation Property اللي عملنا لها Include في الـ Repo
                ReviewerName = x.User?.UserName ?? "مستخدم",
                Rating = x.Rating ?? 0,
                ReviewComment = x.Review,
                CreatedAt = x.InteractionDate
            }).ToList();
        }

        public async Task<ReviewStatsDto> GetProductStatsAsync(int productId)
        {
            var count = await _unitOfWork.UserInteractions.GetTotalCountByProductIdAsync(productId);
            var average = await _unitOfWork.UserInteractions.GetAverageRatingByProductIdAsync(productId);

            return new ReviewStatsDto
            {
                TotalReviews = count,
                AverageRating = Math.Round(average, 1)
            };
        }
        public async Task<ReviewStatsDto> GetRawMaterialStatsAsync(int rawMaterialId)
        {
            var count = await _unitOfWork.UserInteractions.GetTotalCountByRawMaterialIdAsync(rawMaterialId);
            var average = await _unitOfWork.UserInteractions.GetAverageRatingByRawMaterialIdAsync(rawMaterialId);

            return new ReviewStatsDto
            {
                TotalReviews = count,
                AverageRating = Math.Round(average, 1)
            };
        }
    }
}
