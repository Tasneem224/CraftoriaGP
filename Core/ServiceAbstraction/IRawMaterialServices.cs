using Shared.ProductModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IRawMaterialServices
    {

        Task<IEnumerable<ReturnProductDto>> GetAllMaterialsAsync();
        Task<IEnumerable<ReturnProductDto>> GetAllMaterialsOfSpecifiUserAsync(string id);

        Task<ReturnProductDto> GetMaterialsByIdAsync(int id);
        Task<ReturnProductDto> AddMaterialsAsync(CreateProductDto dto, string sellerId);
        Task<ReturnProductDto> UpdateMaterialsAsync(int id, UpdateProductDto dto);
        Task<bool> DeleteMaterialsAsync(int id);
    }
}
