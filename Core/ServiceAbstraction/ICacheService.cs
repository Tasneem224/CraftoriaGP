using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface ICacheService
    {
        Task<string?> GetCacheValueAsync<T>(string key);
        Task SetCacheValueAsync(string key, object value, TimeSpan timeToLive);
    }
}
