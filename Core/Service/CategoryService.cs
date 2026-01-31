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
        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
           var repo= _unitOfWork.GetRepository<Category, int>();
              var categories=await repo.GetAllAsync();
            return categories.Select(c => new CategoryDto
            {
                Id=c.Id,
                Name=c.Name,
                image=c.image
            });
        }
    }
}
