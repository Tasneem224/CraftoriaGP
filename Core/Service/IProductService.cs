using Shared.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IProductService
    {

        public Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
        public Task<ProductInfoDTO> GetAllProductsByIdAsync(int id);
        public Task<UpdateProductDto> CreateProductAsync(UpdateProductDto createProductDto);
        public Task<IEnumerable<UpdateProductDto>> UpdateProductAsync(int id,UpdateProductDto updateProductDto);
        public Task<bool> DeleteProductsAsync(int id);
    }
}
