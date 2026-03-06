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

        Task<int> SaveChanges();

    }
}
