using DomainLayer.Models.CartModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Contracts
{
    public interface ICacheRepository
    {
        Task<T?> GetAsync<T>(string id);
        Task<T?> AddOrUpdateAsync<T>(string id,T entity, TimeSpan? timeToLive = null);
        Task<bool> DeleteAsync(string id);
    }
}
