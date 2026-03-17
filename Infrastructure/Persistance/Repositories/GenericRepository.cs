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
    public class GenericRepository<TEntity, TKey>(StoreDbContext _dbContext) : IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        public async Task AddAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
        }

        public void Remove(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbContext.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(TKey id)
        {
            //var result =await _dbContext.FindAsync(id);
            var result = await _dbContext.Set<TEntity>().FindAsync(id);

            return result;

        }

        public void Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);

        }
        public async Task<TEntity?> GetFirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }

        public IQueryable<TEntity> GetAllQueryable()
        {
            return _dbContext.Set<TEntity>().AsNoTracking();
        }
    }
}
