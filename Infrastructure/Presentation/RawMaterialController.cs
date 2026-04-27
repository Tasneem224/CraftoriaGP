using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Attributes;
using Presentation.Controllers;
using Service;
using ServiceAbstraction;
using Shared;
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
        public async Task<ActionResult<ApiResponse<int>>> GetMyMaterialsCount(string userId)
        {
            var count = await _materialService.GetMaterialsCountByUserIdAsync(userId);
            return Ok(new { totalRawMaterial = count });
        }
        [RedisCache(200)]

        [HttpGet("GetAllMaterials")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReturnProductDto>>>> GetAllMaterials()
        {
            var products = await _materialService.GetAllMaterialsAsync();
            return Ok(products);
        }

        [RedisCache(120)]

        [HttpPost("GetRawMaterialOfSpecificUser")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReturnProductDto>>>> GetAllMaterialsOfSpecifiUser(string userId)
        {
            var products = await _materialService.GetAllMaterialsOfSpecifiUserAsync(userId);
            return Ok(products);
        }

        [HttpGet("GetRawMaterialDetails")]
        public async Task<ActionResult<ApiResponse<ReturnProductDto>>> GetById(int id)
        {
            var product = await _materialService.GetMaterialsByIdAsync(id);
            return Ok(product);
        }

        [Authorize]
        [HttpPost("CreateRawMaterial")]
        public async Task<ActionResult<ApiResponse<ReturnProductDto>>> Create([FromForm] CreateProductDto dto)
        {

            var result = await _materialService.AddMaterialsAsync(dto);
            return Ok(new { message = "Created Successfully", data = result });
        }
        [Authorize]

        [HttpPut("UpdateRawMaterial")]
        public async Task<ActionResult<ApiResponse<ReturnProductDto>>> Update(int RawMaterialid, [FromForm] UpdateProductDto dto)
        {

                var result = await _materialService.UpdateMaterialsAsync(RawMaterialid, dto);
                return Ok(new { message = "Updated Successfully", data = result });
           
        }
        [Authorize]
        [HttpDelete("DeleteRawMaterial")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int RawMaterialid)
        {
            var success = await _materialService.DeleteMaterialsAsync(RawMaterialid);
            return Ok(new { message = "Deleted Successfully" });
        }
        [RedisCache(200)]

        [HttpGet("GetAllRawMaterialCategories")]
        public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetAllMaterialsCategories()
        {
            var result = await _categoryService.GetAllMaterialsCategoriesAsync();
            return SendSuccessResponse<IEnumerable<CategoryDto>>(result, "Categories are returned successfully");
        }


        [HttpGet("GetAllRawMaterialsCategoriesById")]
        public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetAllMaterialsCategoriesById(int id)
        {
            var result = await _categoryService.GetAllMaterialsCategoriesByIdAsync(id);
            return SendSuccessResponse<IEnumerable<CategoryDto>>(result, "Categories are returned successfully");

        }
    }
}
