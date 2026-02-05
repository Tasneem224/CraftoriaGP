using DomainLayer.Models.Interaction;
using Shared.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IInteractionService
    {
        Task<double> GetAverageRatingAsync(string targetId, InteractionTargetType2 type);
        Task<InteractionResultDto> AddOrUpdateAsync(InteractionDto dto);
        Task<int> GetReviewCountAsync(string targetId, InteractionTargetType2 type);
        Task<bool> DeleteAsync(string userId, string targetId, InteractionTargetType2 type);
        Task<IEnumerable<InteractionResultDto>> GetAllAsync(string targetId, InteractionTargetType2 type);
    }
}
