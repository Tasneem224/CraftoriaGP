using DomainLayer.Contracts;
using DomainLayer.Models.Identity;
using DomainLayer.Models.Interaction;
using Microsoft.AspNetCore.Identity;
using ServiceAbstraction;
using Shared.Account;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public UserInteractionService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task<ReviewDto> AddOrUpdateReviewAsync(string userId, AddReviewDto dto)
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
            await _unitOfWork.SaveChanges();
            var user = await _userManager.FindByIdAsync(userId);
            string reviewerName = user?.UserName ?? "Unknown User";
            return new ReviewDto
            {
                InteractionId = interaction.Id, 
                ReviewerId = userId,
                ReviewerName = reviewerName,
                Rating = interaction.Rating ?? 0,
                ReviewComment = interaction.Review,
                CreatedAt = interaction.InteractionDate


            };
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
            var reviews = await _unitOfWork.UserInteractions.GetAllReviewsOfProducBytIdAsync(productId);
            return MapToDto(reviews);
        }
        public async Task<List<ReviewDto>> GetRawMaterialReviewsAsync(int rawMaterialId)
        {
            var reviews = await _unitOfWork.UserInteractions.GetAllReviewsOfRawMaterialByIdAsync(rawMaterialId);
            return MapToDto(reviews);
        }
        public async Task<List<AllReviewsOfTargetUser>> GetUserReviewsAsync(string targetUserId)
        {
            var reviews =  _unitOfWork
                  .GetRepository<UserInteraction, int>()
                  .GetAllQueryable()
                  .Where(r =>
                      r.TargetUserId == targetUserId
                      || (r.Product != null && r.Product.SellerId == targetUserId)
                      || (r.RawMaterial != null && r.RawMaterial.supplierId == targetUserId)
                  ).Select(x => new AllReviewsOfTargetUser
                                   {
                            InteractionId = x.Id,//review id

                            ReviewerId = x.UserId,

                              ReviewerName = x.User.FirstName+" " + x.User.SecondName ?? "مستخدم",
                      Rating = x.Rating ?? 0,

                            ReviewerImage=x.User!.ProfileImage,
                            ReviewComment = x.Review,
                            ItemId= x.ProductId?? x.RawMaterialId,
                            CreatedAt = x.InteractionDate
                        }).ToList();
            return reviews;
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
        public async Task<ReviewStatsDto> GetProductStatsAsync(int productId)
        {
            var count = await _unitOfWork.UserInteractions.GetTotalCountByProductIdAsync(productId);
            var average = await _unitOfWork.UserInteractions.GetAverageRatingByProductIdAsync(productId);

            return new ReviewStatsDto
            {
                TotalReviews = count,
                AverageRating = average
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



        private List<ReviewDto> MapToDto(IEnumerable<UserInteraction> list)
        {
            return list.Select(x => new ReviewDto
            {
                InteractionId = x.Id,
                ReviewerId = x.UserId,
                ReviewerName = x.User?.FirstName + x.User?.SecondName ?? "مستخدم",
                Rating = x.Rating ?? 0,
                ReviewComment = x.Review,
                CreatedAt = x.InteractionDate
            }).ToList();
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
    }
}
