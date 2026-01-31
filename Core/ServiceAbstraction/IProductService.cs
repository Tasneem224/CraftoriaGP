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
        Task<ReturnProductDto> GetProductByIdAsync(int id);
        Task<ReturnProductDto> AddProductAsync(CreateProductDto dto, string sellerId);
        Task<ReturnProductDto> UpdateProductAsync(int id, UpdateProductDto dto, string sellerId);
        Task<bool> DeleteProductAsync(int id);
    }
}
