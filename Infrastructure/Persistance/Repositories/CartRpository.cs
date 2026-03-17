using DomainLayer.Contracts;
using DomainLayer.Models.CartModule;
using Microsoft.AspNetCore.Http.HttpResults;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class CartRpository(IConnectionMultiplexer _connection) : ICartRepository
    {

        private readonly IDatabase _database = _connection.GetDatabase();
        public async Task<T?> AddOrUpdateAsync<T>(string key,T entity, TimeSpan? timeToLive = null)
        {
            var json=JsonSerializer.Serialize(entity);
             var created= await _database.StringSetAsync(key, json, timeToLive??TimeSpan.FromDays(3));
            if (created)
            {
                return entity;
            }
            return default;
        }

        public async Task<int> Count(string id)
        {
            var result = await _database.StringGetAsync(id);

            if (result.IsNullOrEmpty)
            {
                return 0;
            }

            var basket = JsonSerializer.Deserialize<CustomerCart>(result);
            return basket?.cartItems?.Count ?? 0;
        }

        public async Task<bool> DeleteAsync(string id)=> await _database.KeyDeleteAsync(id);
        
        
        public async Task<T?> GetAsync<T>(string id)

        {
            var result = await _database.StringGetAsync(id);
            if(result.IsNullOrEmpty) return default;
            return JsonSerializer.Deserialize<T>(result!);
        }
    }
}
