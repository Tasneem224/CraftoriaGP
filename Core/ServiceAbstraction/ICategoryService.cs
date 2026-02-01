using Shared.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllProductCategoriesAsync();
        Task<IEnumerable<CategoryDto>> GetAllProductCategoriesByIdAsync(int id);
        Task<IEnumerable<CategoryDto>> GetAllMaterialsCategoriesAsync();
        Task<IEnumerable<CategoryDto>> GetAllMaterialsCategoriesByIdAsync(int id);
    }
}
