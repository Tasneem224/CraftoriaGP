using DomainLayer.Models.Identity;
using DomainLayer.Models.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Contracts
{
    public interface IUnitOfWork
    {
        IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>;
        IUserInteractionRepository UserInteractions { get; }
        IFavouriteRepository Favourites { get; }
        ISessionRepository Sessions { get; } // الـ Repo الجديد
        IOrderRepository Orders { get; }
        IChatBotSessionRepository ChatBot { get; }

        /// <summary>
        /// Chat-message repository — provides all inbox / conversation / read-receipt
        /// queries that go beyond the capabilities of the generic repository.
        /// </summary>
        IMessageRepository Messages { get; }


        IGenericRepository<VendorWallet, int> VendorWallets { get; }
        IAdminPanelRepo AdminPanel { get; }
        Task<int> SaveChangesAsync();

    }
}
