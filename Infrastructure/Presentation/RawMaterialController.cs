using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Controllers;
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

namespace Presentation
{
    public class RawMaterialController(IRawMaterialServices _materialService, ICategoryService _categoryService):BaseApiController
    {
        [HttpGet("my-Material-count")]
        public async Task<IActionResult> GetMyMaterialsCount(string userId)
        {
            var count = await _materialService.GetMaterialsCountByUserIdAsync(userId);
            return Ok(new { totalRawMaterial = count });
        }

        [HttpGet("GetAllMaterials")]
        public async Task<IActionResult> GetAllMaterials()
        {
            var products = await _materialService.GetAllMaterialsAsync();
            return Ok(products);
        }
        [HttpPost("GetRawMaterialOfSpecificUser")]
        public async Task<IActionResult> GetAllMaterialsOfSpecifiUser(string userId)
        {
            var products = await _materialService.GetAllMaterialsOfSpecifiUserAsync(userId);
            return Ok(products);
        }

        [HttpGet("GetRawMaterialById")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _materialService.GetMaterialsByIdAsync(id);
            return Ok(product);
        }

        [Authorize]
        [HttpPost("CreateRawMaterial")]
        public async Task<IActionResult> Create([FromForm] CreateProductDto dto)
        {

            var result = await _materialService.AddMaterialsAsync(dto);
            return Ok(new { message = "Created Successfully", data = result });
        }
        [Authorize]

        [HttpPut("UpdateRawMaterial")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateProductDto dto)
        {

                var result = await _materialService.UpdateMaterialsAsync(id, dto);
                return Ok(new { message = "Updated Successfully", data = result });
           
        }
        [Authorize]
        [HttpDelete("DeleteRawMaterial")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _materialService.DeleteMaterialsAsync(id);
            return Ok(new { message = "Deleted Successfully" });
        }

        [HttpGet("GetAllRawMaterialCategories")]
        public async Task<IActionResult> GetAllMaterialsCategories()
        {
            var result = await _categoryService.GetAllMaterialsCategoriesAsync();
            return SendSuccessResponse<IEnumerable<CategoryDto>>(result, "Categories are returned successfully");
        }


        [HttpPost("GetAllRawMaterialsCategoriesById")]
        public async Task<IActionResult> GetAllMaterialsCategoriesById(int id)
        {
            var result = await _categoryService.GetAllMaterialsCategoriesByIdAsync(id);
            return SendSuccessResponse<IEnumerable<CategoryDto>>(result, "Categories are returned successfully");

        }
    }
}
