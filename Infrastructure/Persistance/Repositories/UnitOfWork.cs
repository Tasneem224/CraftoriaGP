using DomainLayer.Contracts;
using DomainLayer.Models.Items;
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

        public UnitOfWork(StoreDbContext context)
        {
            _dbContext = context;
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

        public async Task<int> SaveChanges()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
