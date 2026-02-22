using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Presentation.Controllers;
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

namespace Presentation
{
    public class RawMaterialController(IRawMaterialServices _materialService, ICategoryService _categoryService, IStringLocalizer<SharedResources> _stringLocalizer) :BaseApiController
    {
        [HttpGet("my-Material-count")]
        [Authorize]
        public async Task<IActionResult> GetMyMAterialsCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId)) return Unauthorized();

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
            if (product == null) return NotFound(new { message = _stringLocalizer[SharedResourcesKeys.RawMaterialNotFound] });
            return Ok(product);
        }

        [HttpPost("CreateRawMaterial")]
        public async Task<IActionResult> Create([FromForm] CreateProductDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _materialService.AddMaterialsAsync(dto);
            return Ok(new { message = _stringLocalizer[SharedResourcesKeys.CreatedSuccessfully], data = result });
        }

        [HttpPut("UpdateRawMaterial")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateProductDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);


            try
            {
                var result = await _materialService.UpdateMaterialsAsync(id, dto);
                if (result == null) return NotFound(new { message = _stringLocalizer[SharedResourcesKeys.RawMaterialNotFound] });

                return Ok(new { message = _stringLocalizer[SharedResourcesKeys.UpdatedSuccessfully], data = result });
            }
            catch (System.Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpDelete("DeleteRawMaterial")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _materialService.DeleteMaterialsAsync(id);
            if (!success) return NotFound(_stringLocalizer[SharedResourcesKeys.ProductNotFound]);

            return Ok(new { message = _stringLocalizer[SharedResourcesKeys.DeletedSuccessfully] });
        }
        [HttpGet("GetAllRawMaterialCategories")]
        public async Task<IActionResult> GetAllMaterialsCategories()
        {
            var result = await _categoryService.GetAllMaterialsCategoriesAsync();
            return SendSuccessResponse<IEnumerable<CategoryDto>>(result, _stringLocalizer[SharedResourcesKeys.CategoriesReturnedSuccessfully]);
        }


        [HttpPost("GetAllRawMaterialsCategoriesById")]
        public async Task<IActionResult> GetAllMaterialsCategoriesById(int id)
        {
            var result = await _categoryService.GetAllMaterialsCategoriesByIdAsync(id);
            return SendSuccessResponse<IEnumerable<CategoryDto>>(result, _stringLocalizer[SharedResourcesKeys.CategoriesReturnedSuccessfully]);

        }
    }
}
