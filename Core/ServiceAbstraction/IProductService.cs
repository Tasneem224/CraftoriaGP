using Shared.ProductModule;
using Shared.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IProductService
    {
        Task<IEnumerable<ReturnProductDto>> GetAllProductsAsync();
        Task<IEnumerable<ReturnProductsOfCategory>> GetAllProductsOfSpecificCategory(int id);
        Task<IEnumerable<ReturnProductDto>> GetAllProductsOfSpecifiUserAsync(string id);

        Task<ReturnProductDto> GetProductByIdAsync(int id);
        Task<ReturnProductDto> AddProductAsync(CreateProductDto dto);
        Task<ReturnProductDto> UpdateProductAsync(int id, UpdateProductDto dto);
        Task<bool> DeleteProductAsync(int id);
        Task<int> GetProductsCountByUserIdAsync(string userId);
        Task<List<ReturnSearchDto>> SearchProductsAsync(string query);
    }
}
