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
        Task<InteractionResultDto> AddOrUpdateAsync(InteractionDto dto);
        Task<bool> DeleteAsync(string userId, string targetId, InteractionTargetType type);
        Task<IEnumerable<InteractionResultDto>> GetAllAsync(string targetId, InteractionTargetType type);
    }
}
