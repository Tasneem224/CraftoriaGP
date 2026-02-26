using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface ICacheService
    {
        Task<string?> GetAsync<T>(string key);
        Task SetAsync(string key, object value, TimeSpan timeToLive);
    }
}
