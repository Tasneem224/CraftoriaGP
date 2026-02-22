using DomainLayer.Models.Items;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Service;
using ServiceAbstraction;
using Shared.Category;
using Shared.ProductModule;
using Shared.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    public class ProductsController(IProductService _productService, ICategoryService _categoryService, IStringLocalizer<SharedResources> _stringLocalizer) : BaseApiController
    {
        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }
        [HttpGet("GetProductsOfSpecificUser")]
        public async Task<IActionResult> GetProductsOfSpecificUser(string userId)
        {
            var products = await _productService.GetAllProductsOfSpecifiUserAsync(userId);
            return Ok(products);
        }

        [HttpGet("GetProductDetailsById")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound(new { message = _stringLocalizer[SharedResourcesKeys.ProductNotFound] });
            return Ok(product);
        }

        [Authorize]
        [HttpPost("CreateProduct")]
        public async Task<IActionResult> Create([FromForm] CreateProductDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _productService.AddProductAsync(dto);
            return Ok(new { message = _stringLocalizer[SharedResourcesKeys.CreatedSuccessfully], data = result });
        }

        [HttpGet("my-products-count")]
        public async Task<IActionResult> GetMyProductsCount(string userId)
        {

            var count = await _productService.GetProductsCountByUserIdAsync(userId);
            return Ok(new { totalProducts = count });
        }

        [Authorize]
        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateProductDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);


            try
            {
                var result = await _productService.UpdateProductAsync(id, dto);
                if (result == null) return NotFound(new { message = _stringLocalizer[SharedResourcesKeys.ProductNotFound] });

                return Ok(new { message = _stringLocalizer[SharedResourcesKeys.UpdatedSuccessfully], data = result });
            }
            catch (System.Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
        [Authorize]
        [HttpDelete("DeleteProduct")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _productService.DeleteProductAsync(id);
            if (!success) return NotFound(_stringLocalizer[SharedResourcesKeys.ProductNotFound]);

            return Ok(new { message = _stringLocalizer[SharedResourcesKeys.DeletedSuccessfully] });
        }
        [HttpGet("GetAllProductCategories")]
        public async Task<IActionResult> GetAllProductCategories()
        {
            var result = await _categoryService.GetAllProductCategoriesAsync();
            return SendSuccessResponse<IEnumerable<CategoryDto>>(result, _stringLocalizer[SharedResourcesKeys.CategoriesReturnedSuccessfully]);
        }

      
        [HttpGet("GetAllProductsOfSpecificCategory")]
        public async Task<IActionResult> GetAllProductCategoriesById(int id)
        {
            var result = await _productService.GetAllProductsOfSpecificCategory(id);
            return SendSuccessResponse<IEnumerable<ReturnProductsOfCategory>>(result, _stringLocalizer[SharedResourcesKeys.CategoriesReturnedSuccessfully]);

        }
       
    }
}
