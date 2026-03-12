using Shared.TopRated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface ITopRatedService
    {
        Task<List<TopProductsDto>> GetTopProductsAsync(int count = 5);
        Task<List<TopSellersDto>> GetTopSellersAsync(int count = 5);
        Task<List<TopRawMaterialsDto>> GetTopRawMaterialsAsync(int count);
        Task<List<TopSellersDto>> GetTopSellersByRoleAsync(string role, int count = 5);
    }
}
