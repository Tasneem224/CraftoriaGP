using Microsoft.AspNetCore.Mvc;
using Service;
using ServiceAbstraction;
using Shared.Category;
using Shared.ProductModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    public class ProductsController(IProductService _productService, ICategoryService _categoryService) : BaseApiController
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

        [HttpGet("GetProductById")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound(new { message = "Product not found" });
            return Ok(product);
        }

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> Create([FromForm] CreateProductDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _productService.AddProductAsync(dto, dto.SellerId);
            return Ok(new { message = "Created Successfully", data = result });
        }

        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateProductDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);


            try
            {
                var result = await _productService.UpdateProductAsync(id, dto);
                if (result == null) return NotFound(new { message = "Product not found" });

                return Ok(new { message = "Updated Successfully", data = result });
            }
            catch (System.Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpDelete("DeleteProduct")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _productService.DeleteProductAsync(id);
            if (!success) return NotFound("Product not found");

            return Ok(new { message = "Deleted Successfully" });
        }
        [HttpGet("GetAllProductCategories")]
        public async Task<IActionResult> GetAllProductCategories()
        {
            var result = await _categoryService.GetAllProductCategoriesAsync();
            return SendSuccessResponse<IEnumerable<CategoryDto>>(result, "Categories are returned successfully");
        }

      
        [HttpGet("GetAllProductCategoriesById")]
        public async Task<IActionResult> GetAllProductCategoriesById(int id)
        {
            var result = await _categoryService.GetAllProductCategoriesByIdAsync(id);
            return SendSuccessResponse<IEnumerable<CategoryDto>>(result, "Categories are returned successfully");

        }
       
    }
}
