using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Presentation.Controllers;
using ServiceAbstraction;
using Shared;
using Shared.Account;
using Shared.Admin_Panel;
using Shared.Category;
using Shared.IdentityModule;
using Shared.ProductModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    [AllowAnonymous]

    public class AdminPanelController(IAuthenticationService _authService,IServiceManager _serviceManager) : BaseApiController
    {
        #region Auth Admin
        [HttpPost("Admin-Register")]
        public async Task<ActionResult<ApiResponse<ReturnUserDTO>>> RegisterAsync(RegisterAdmintDto admintDto)=>
            SendSuccessResponse(await _authService.RegisterAdminAsync(admintDto));
        [HttpPost("Admin-/Login")]
        public async Task<ActionResult<ApiResponse<ReturnUserDTO>>> LoginAdminAsync(LoginDTO admintDto)=>
            SendSuccessResponse(await _authService.LoginAdminAsync(admintDto));
        #endregion

        #region Products
        [HttpGet("products")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReturnProductDto>>>> GetAllProducts(int pageNumber, int pageSize)
            => SendSuccessResponse(await _serviceManager.AdminPanelService.GetAllProductsPagination(pageNumber, pageSize));

        [HttpGet("products/new")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReturnProductDto>>>> GetNewProducts(int pageNumber, int pageSize)
            => SendSuccessResponse(await _serviceManager.AdminPanelService.GetNewProductsLast30DaysAgo(pageNumber, pageSize));

        [HttpGet("products/count")]
        public async Task<ActionResult<ApiResponse<int>>> GetProductsCount()
            => SendSuccessResponse(await _serviceManager.AdminPanelService.GetCountProductsAsync());

        [HttpGet("products/count/new")]
        public async Task<ActionResult<ApiResponse<int>>> GetNewProductsCount()
            => SendSuccessResponse(await _serviceManager.AdminPanelService.GetCountNewProductsLast30DaysAgo());

        [HttpPut("products/{id}/visibility")]
        public async Task<ActionResult<ApiResponse<bool>>>ToggleProductVisibility(int id, bool isVisible)
            => SendSuccessResponse(await _serviceManager.AdminPanelService.ToggleProductVisibility(id, isVisible));

        [HttpPost("products/AddProductCategory")]
        public async Task<ActionResult<ApiResponse<ReturnAdminCategoriesDto>>> AddProductCategory(AddAdminCategoryDto category)
            => SendSuccessResponse(await _serviceManager.AdminPanelService.AddProductCategoryAsync(category));

        [HttpPut("products/UpdateProductCategory")]
        public async Task<ActionResult<ApiResponse<ReturnAdminCategoriesDto>>> UpdateProductCategory(UpdateAdminCategoryDto category)
            => SendSuccessResponse(await _serviceManager.AdminPanelService.UpdateProductCategoryAsync( category));

        [HttpDelete("products/DeleteProductCategory")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteProductCategory(int id)
            => SendSuccessResponse(await _serviceManager.AdminPanelService.DeleteProductCategory(id));
        #endregion

        #region Materials
        [HttpGet("GetAllMaterials")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReturnProductDto>>>> GetAllMaterials(int pageNumber, int pageSize)
            => SendSuccessResponse(await _serviceManager.AdminPanelService.GetAllRawMaterialPagination(pageNumber, pageSize));

        [HttpGet("materials/GetNewMaterials")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReturnProductDto>>>> GetNewMaterials(int pageNumber, int pageSize)
            => SendSuccessResponse(await _serviceManager.AdminPanelService.GetNewMaterialsLast30DaysAgo(pageNumber, pageSize));

        [HttpGet("materials/GetMaterialsCount")]
        public async Task<ActionResult<ApiResponse<int>>> GetMaterialsCount()
            => SendSuccessResponse(await _serviceManager.AdminPanelService.GetCountMaterialsAsync());

        [HttpGet("materials/GetNewMaterialsCount")]
        public async Task<ActionResult<ApiResponse<int>>> GetNewMaterialsCount()
            => SendSuccessResponse(await _serviceManager.AdminPanelService.GetCountNewMaterialsLast30Days());

        [HttpPut("materials/ToggleMaterialVisibility")]
        public async Task<ActionResult<ApiResponse<bool>>> ToggleMaterialVisibility(int id, bool isVisible)
            => SendSuccessResponse(await _serviceManager.AdminPanelService.ToggleRawMaterialVisibility(id, isVisible));

        [HttpPost("materials/AddMaterialCategory")]
        public async Task<ActionResult<ApiResponse<ReturnAdminCategoriesDto>>> AddMaterialCategory(AddAdminCategoryDto category)
            => SendSuccessResponse(await _serviceManager.AdminPanelService.AddRawMaterialCategoryAsync(category));

        [HttpPut("materials/UpdateMaterialCategory")]
        public async Task<ActionResult<ApiResponse<ReturnAdminCategoriesDto>>> UpdateMaterialCategory( UpdateAdminCategoryDto category)
            => SendSuccessResponse(await _serviceManager.AdminPanelService.UpdateRawMaterialCategory( category));

        [HttpDelete("materials/DeleteMaterialCategory")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteMaterialCategory(int id)
            => SendSuccessResponse(await _serviceManager.AdminPanelService.DeleteRawMaterialCategory(id));
        #endregion

        #region Users
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ReturnAccountDto>>>> GetAllUsers(int pageNumber, int pageSize)
            => SendSuccessResponse(await _serviceManager.AdminPanelService.GetAllUsersAsync(pageNumber, pageSize));

        [HttpGet("users/GetUserById")]
        public async Task<ActionResult<ApiResponse<ReturnAccountDto>>> GetUserById(string id)
            => SendSuccessResponse(await _serviceManager.AdminPanelService.GetUserByIdAsync(id));

        [HttpGet("users/count")]
        public async Task<ActionResult<ApiResponse<int>>> GetTotalUsersCount()
            => SendSuccessResponse(await _serviceManager.AdminPanelService.GetTotalUsersCount());

        [HttpGet("users/GetNewUsersCount")]
        public async Task<IActionResult> GetNewUsersCount()
            => SendSuccessResponse(await _serviceManager.AdminPanelService.GetCountNewUsersLast30DaysAsync());

        [HttpDelete("users/DeleteUser")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(string id)
            => SendSuccessResponse(await _serviceManager.AdminPanelService.DeleteUser(id));

        [HttpPut("users/ToggleUserBlockStatus")]
        public async Task<IActionResult> ToggleUserBlockStatus(string id, bool isBlocked)
            => SendSuccessResponse( _serviceManager.AdminPanelService.ToggleUserBlockStatus(id, isBlocked));

        [HttpPut("users/UpgradeUserRole")]
        public async Task<IActionResult> UpgradeUserRole(string id)
            => SendSuccessResponse(await _serviceManager.AdminPanelService.UpgradeUsersInRole(id));
        #endregion

        #region Orders
        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders()
            => SendSuccessResponse(await _serviceManager.AdminPanelService.GetAllOrdersAsync());

        [HttpGet("orders/{orderId}")]
        public async Task<IActionResult> GetOrderDetails(Guid orderId)
            => SendSuccessResponse(await _serviceManager.AdminPanelService.GetOrderDetails(orderId));

        [HttpGet("orders/GetPendingOrdersCount")]
        public async Task<IActionResult> GetPendingOrdersCount()
            => SendSuccessResponse(await _serviceManager.AdminPanelService.GetPendingOrdersCountAsync());

        [HttpPut("orders/UpdateOrderStatus")]
        public async Task<IActionResult> UpdateOrderStatus(Guid orderId, string status)
            => SendSuccessResponse(await _serviceManager.AdminPanelService.UpdateOrderStatusAsync(orderId, status));
        #endregion

        #region Reviews
        [HttpGet("GetRecentReviews")]
        public async Task<IActionResult> GetRecentReviews(int count, int days = 100)
            => SendSuccessResponse(await _serviceManager.AdminPanelService.GetRecentReviewsAsync(count, days));

        [HttpDelete("reviews/DeleteReview")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteReview(int reviewId)
            => SendSuccessResponse(await _serviceManager.AdminPanelService.DeleteReview(reviewId));
        #endregion
    }
}


