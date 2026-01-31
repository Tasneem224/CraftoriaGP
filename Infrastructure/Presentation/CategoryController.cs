using DomainLayer.Models.Categories;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers;
using ServiceAbstraction;
using Shared.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    public class CategoryController:BaseApiController
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService) { 

            _categoryService = categoryService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
           var result=  await   _categoryService.GetAllCategoriesAsync();
            return SendSuccessResponse<IEnumerable<CategoryDto>>(result, "Categories are returned successfully");
        }
    }
}
