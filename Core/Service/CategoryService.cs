using DomainLayer.Contracts;
using DomainLayer.Models.Categories;
using ServiceAbstraction;
using Shared.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService(IUnitOfWork unitOfWork)
        { 
            _unitOfWork = unitOfWork;
        
        }
        public async Task<IEnumerable<CategoryDto>> GetAllProductCategoriesAsync()
        {
           var repo= _unitOfWork.GetRepository<ProductCategory, int>();
              var categories=await repo.GetAllAsync();
            return categories.Select(c => new CategoryDto
            {
                Id=c.Id,
                Name=c.Name,
                image=c.image
            });
        }

        public async Task<IEnumerable<CategoryDto>> GetAllProductCategoriesByIdAsync(int id)
        {
            var repo =_unitOfWork.GetRepository<ProductCategory, int>();
            var categories =await repo.GetAllAsync();
            var specificCategories = categories.Where(i => i.Id == id);
            return specificCategories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                image = c.image
            });
        }
        public async Task<IEnumerable<CategoryDto>> GetAllMaterialsCategoriesAsync()
        {
           var repo= _unitOfWork.GetRepository<Raw_Category_Material, int>();
              var categories=await repo.GetAllAsync();
            return categories.Select(c => new CategoryDto
            {
                Id=c.Id,
                Name=c.Name,
                image=c.image
            });
        }
        public async Task<IEnumerable<CategoryDto>> GetAllMaterialsCategoriesByIdAsync(int id)
        {
            var repo =_unitOfWork.GetRepository<Raw_Category_Material, int>();
            var categories =await repo.GetAllAsync();
            var specificCategories = categories.Where(i => i.Id == id);
            return specificCategories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                image = c.image
            });
        }
    }
}
