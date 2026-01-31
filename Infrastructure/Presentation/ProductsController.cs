using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction;
using Shared.ProductModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    public class ProductsController(IProductService _productService) : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound(new { message = "Product not found" });
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateProductDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);            

            var result = await _productService.AddProductAsync(dto,dto.SellerId);
            return Ok(new { message = "Created Successfully", data = result });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateProductDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = "test-user-id";

            try
            {
                var result = await _productService.UpdateProductAsync(id, dto, userId);
                if (result == null) return NotFound(new { message = "Product not found" });

                return Ok(new { message = "Updated Successfully", data = result });
            }
            catch (System.Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _productService.DeleteProductAsync(id);
            if (!success) return NotFound("Product not found");

            return Ok(new { message = "Deleted Successfully" });
        }

    }
}
