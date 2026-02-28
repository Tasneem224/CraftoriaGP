using DomainLayer.Contracts;
using ServiceAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class CacheService : ICacheService
    {
        private readonly ICacheRepository _cacheRepository;
        public CacheService(ICacheRepository cacheRepository)
        {
            _cacheRepository = cacheRepository;
        }
        public Task<string?> GetCacheValueAsync<T>(string key)=>
             _cacheRepository.GetAsync(key);
        

        public Task SetCacheValueAsync(string key, object value, TimeSpan timeToLive)=>
                        _cacheRepository.SetAsync(key, value, timeToLive );

    }
}
