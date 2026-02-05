using DomainLayer.Models.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Contracts
{
    public interface IUserInteractionRepository
    {
        Task<UserInteraction?> GetAsync(string userId,string targetId,InteractionTargetType targetType);

        Task<IEnumerable<UserInteraction>> GetAllForTargetAsync(string targetId, InteractionTargetType targetType);


        Task AddAsync(UserInteraction interaction);
        void Update(UserInteraction interaction);

        void Remove(UserInteraction interaction);

    }
}
