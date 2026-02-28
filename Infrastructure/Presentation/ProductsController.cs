using DomainLayer.Models.Items;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Attributes;
using Service;
using ServiceAbstraction;
using Shared.Category;
using Shared.ProductModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    public class ProductsController(IProductService _productService, ICategoryService _categoryService) : BaseApiController
    {

        [RedisCache(200)]
        [HttpGetAttribute("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
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
        [RedisCache(120)]
        [HttpGet("GetProductDetailsById")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            return Ok(product);
        }
        [Authorize]
        [HttpPost("CreateProduct")]
        public async Task<IActionResult> Create([FromForm] CreateProductDto dto)
        {

            var result = await _productService.AddProductAsync(dto);
            return Ok(new { message = "Created Successfully", data = result });
        }
        [RedisCache(200)]
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
           
                var result = await _productService.UpdateProductAsync(id, dto);
                return Ok(new { message = "Updated Successfully", data = result });
          
        }
        [Authorize]
        [HttpDelete("DeleteProduct")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _productService.DeleteProductAsync(id);
            return Ok(new { message = "Deleted Successfully" });
        }
        [RedisCache(200)]

        [HttpGet("GetAllProductCategories")]
        public async Task<IActionResult> GetAllProductCategories()
        {
            var result = await _categoryService.GetAllProductCategoriesAsync();
            return SendSuccessResponse(result, "Categories are returned successfully");
        }
        [RedisCache(120)]

        [HttpGet("GetAllProductsOfSpecificCategory")]
        public async Task<IActionResult> GetAllProductCategoriesById(int id)
        {
            var result = await _productService.GetAllProductsOfSpecificCategory(id);
            return SendSuccessResponse(result, "Categories are returned successfully");

        }
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return BadRequest("Search query cannot be empty");

            var results = await _productService.SearchProductsAsync(query);
            return Ok(results);
        }
    }
}
