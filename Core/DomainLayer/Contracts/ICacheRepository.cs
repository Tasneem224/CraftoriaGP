using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Contracts
{
    public interface ICacheRepository
    {
        //Get=> Already cached [return data]
        Task<string?> GetAsync(string key);
        //set => no caching happen [first time to call endpoint]=> cache the data with specific key and value
        Task SetAsync(string key,object value, TimeSpan timeToLive);
    }
}
