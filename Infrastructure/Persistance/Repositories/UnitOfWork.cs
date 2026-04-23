using DomainLayer.Contracts;
using DomainLayer.Models.Identity;
using DomainLayer.Models.Items;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistance.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        public IUserInteractionRepository UserInteractions { get; }


        public IFavouriteRepository Favourites { get; }


        public ISessionRepository Sessions { get; } // الـ Repo الجديد
        public IOrderRepository Orders { get; } 

        public IChatBotSessionRepository ChatBot { get; }
        public IMessageRepository Messages { get; }

        public IGenericRepository<VendorWallet, int> VendorWallets { get; }

        public IAdminPanelRepo AdminPanel { get; }

        public UnitOfWork(StoreDbContext context,UserManager<ApplicationUser> userManager)
        {
            _dbContext = context;
            _userManager = userManager;
            UserInteractions = new UserInteractionRepository(_dbContext);
            Favourites = new FavouriteRepository(_dbContext);
            Sessions = new SessionRepository(_dbContext); // عمل الـ Instance
            Orders= new OrderRepository(_dbContext); 
            ChatBot = new ChatBotSessionRepository(_dbContext);
            VendorWallets = new GenericRepository<VendorWallet, int>(_dbContext);
            AdminPanel = new AdminPanelRepo(_dbContext, _userManager);
            Messages = new MessageRepository(_dbContext);



        }
        private readonly Dictionary<string, object> _repositories = new Dictionary<string, object>();


        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {

            var typeName = typeof(TEntity).Name;
            if (_repositories.ContainsKey(typeName))
                return (IGenericRepository<TEntity, TKey>)_repositories[typeName];
            var repo = new GenericRepository<TEntity, TKey>(_dbContext);
            _repositories[typeName] = repo;
            return (repo);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
