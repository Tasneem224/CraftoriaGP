using Shared.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IInteractionService<T>
    {
       Task<ReturnInteractionDto<T>> AddOrUpdateInteraction(InteractionDto<T> interactionDto);
        Task<bool> DeleteInteractionAsync(string userId, T ItemOrUserId);
        Task<IEnumerable<ReturnInteractionDto<T>>> GetReviewsByTargetId(T id);



    }
}
