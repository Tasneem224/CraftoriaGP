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
    internal class InteractionService : IInteractionService
    {
        private readonly IUnitOfWork _unitOfWork;
        public InteractionService(IUnitOfWork unitOfWork) {
        
            _unitOfWork = unitOfWork;
        }

        public async Task<InteractionResultDto> AddOrUpdateAsync(InteractionDto dto)
        {
            var interaction = await GetExistingInteractionAsync(dto);

            if (interaction is null)
                interaction = CreateInteraction(dto);
            else
                UpdateInteraction(interaction, dto);

            await _unitOfWork.SaveChanges();

            return MapToReturnDto(interaction);
        }
        public async Task<bool> DeleteAsync(string userId, string targetId, InteractionTargetType2 type)
        {
            var interaction = await _unitOfWork.UserInteractions
                   .GetAsync(userId, targetId, (InteractionTargetType)type);

            if (interaction is null)
                return false;

            _unitOfWork.UserInteractions.Remove(interaction);
            await _unitOfWork.SaveChanges();
            return true;
        }

        public async Task<IEnumerable<InteractionResultDto>> GetAllAsync(string targetId, InteractionTargetType2 type)
        {
            var t =(InteractionTargetType) type;
            var reviews= await _unitOfWork.UserInteractions.GetAllForTargetAsync(targetId, t);

            return reviews.Select(x => new InteractionResultDto
            {
                ItemOrUserId = x.TargetId,
                USerId = x.UserId,
                Rating = x.Rating,
                Review = x.Review,
            });
        }


        public async Task<int> GetReviewCountAsync(string targetId, InteractionTargetType2 type)
        {
            return await _unitOfWork.UserInteractions
                .GetReviewCountAsync(targetId, (InteractionTargetType)type);
        }
        public async Task<double> GetAverageRatingAsync(string targetId, InteractionTargetType2 type)
        {
            return await _unitOfWork.UserInteractions
                .GetAverageRatingAsync(targetId,(InteractionTargetType) type);
        }


        private InteractionResultDto MapToReturnDto(UserInteraction interaction)
        {
            return new InteractionResultDto
            {
                USerId = interaction.UserId,
                ItemOrUserId = interaction.TargetId,
                Review = interaction.Review ?? string.Empty,
                Rating = interaction.Rating ?? 0
            };
        }
        private UserInteraction CreateInteraction(InteractionDto dto)
        {
            var interaction = new UserInteraction
            {
                UserId = dto.UserId,
                TargetId = dto.TargetId,
                TargetType = (InteractionTargetType)dto.TargetType,
                Rating = dto.Rating,
                Review = dto.Review,
                IsFavourite = dto.IsFavourite,
                InteractionDate = DateTime.UtcNow
            };

            _unitOfWork.UserInteractions.AddAsync(interaction);
            return interaction;
        }
        private void UpdateInteraction(UserInteraction interaction, InteractionDto dto)
        {
            interaction.Rating = dto.Rating ?? interaction.Rating;
            interaction.Review = dto.Review ?? interaction.Review;
            interaction.IsFavourite = dto.IsFavourite ?? interaction.IsFavourite;
            interaction.InteractionDate = DateTime.UtcNow;

            _unitOfWork.UserInteractions.Update(interaction);
        }
        private async Task<UserInteraction?> GetExistingInteractionAsync(InteractionDto dto)
        {
            return await _unitOfWork.UserInteractions.GetAsync(
                dto.UserId,
                dto.TargetId,
                 (InteractionTargetType)dto.TargetType);
        }
    }
}
