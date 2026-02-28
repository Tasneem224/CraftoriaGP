using DomainLayer.Contracts;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class CacheRepository(IConnectionMultiplexer _connection) : ICacheRepository
    {
        private readonly IDatabase _database = _connection.GetDatabase();

        public async Task<string?> GetAsync(string key)
        {
          var cache=await  _database.StringGetAsync(key);
            return cache.IsNullOrEmpty ? default : cache;
        }

        public async Task SetAsync(string key, object value, TimeSpan timeToLive )
        {
            //so we need to convert C# obj to json string to store it in redis cache
            var json = JsonSerializer.Serialize(value);

            var cache =await _database.StringSetAsync(key,json, timeToLive);

        }
    }
}
