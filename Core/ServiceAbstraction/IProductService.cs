using Shared.ProductModule;
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
        Task<IEnumerable<ReturnProductDto>> GetAllProductsOfSpecifiUserAsync(string id);

        Task<ReturnProductDto> GetProductByIdAsync(int id);
        Task<ReturnProductDto> AddProductAsync(CreateProductDto dto, string sellerId);
        Task<ReturnProductDto> UpdateProductAsync(int id, UpdateProductDto dto);
        Task<bool> DeleteProductAsync(int id);

    }
}
