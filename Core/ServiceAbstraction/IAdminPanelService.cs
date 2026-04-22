using Shared.Account;
using Shared.Admin_Panel;
using Shared.Interaction;
using Shared.Order;
using Shared.ProductModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IAdminPanelService
    {
        #region Products
        Task<List<ReturnProductDto>> GetNewProductsLast30DaysAgo(int pageNumber, int pageSize);
        Task<List<ReturnProductDto>> GetAllProductsPagination(int pageNumber, int pageSize);
        Task<int> GetCountNewProductsLast30DaysAgo();
        Task<ReturnAdminCategoriesDto> AddProductCategoryAsync(AddAdminCategoryDto category);
        Task<bool> DeleteProductCategory(int id);
        Task<ReturnAdminCategoriesDto> UpdateProductCategoryAsync(UpdateAdminCategoryDto category);
        Task<bool> ToggleProductVisibility(int id, bool isVisible);
        Task<int> GetCountProductsAsync();
        Task<List<object>> GetTopSellingProducts(int count);

        #endregion

        #region Raw Materials
        Task<List<ReturnProductDto>> GetNewMaterialsLast30DaysAgo(int pageNumber, int pageSize);
        Task<List<ReturnProductDto>> GetAllRawMaterialPagination(int pageNumber, int pageSize);
        Task<int> GetCountNewMaterialsLast30Days();
        Task<int> GetCountMaterialsAsync();
        Task<ReturnAdminCategoriesDto> AddRawMaterialCategoryAsync(AddAdminCategoryDto category);
        Task<bool> DeleteRawMaterialCategory(int id);
        Task<ReturnAdminCategoriesDto> UpdateRawMaterialCategory(UpdateAdminCategoryDto category);
        Task<bool> ToggleRawMaterialVisibility(int id, bool isVisible);
        #endregion

        #region Users
        Task<List<ReturnAccountDto>> GetAllUsersAsync(int pageNumber, int pageSize);
        Task<ReturnAccountDto> GetUserByIdAsync(string id);
        Task<bool> DeleteUser(string id);
        Task<bool> UpgradeUsersInRole(string id);
        Task<bool> ToggleUserBlockStatus(string userId, bool isBlocked);
        Task<int> GetTotalUsersCount();
        Task<int> GetCountNewUsersLast30DaysAsync();
        #endregion

        #region sessions
        #endregion

        #region Orders
        Task<List<OrderToReturnDto>> GetAllOrdersAsync();
        Task<OrderToReturnDto> GetOrderDetails(Guid orderId);
        Task<bool> UpdateOrderStatusAsync(Guid orderId, string status);
        Task<int> GetPendingOrdersCountAsync();

        #endregion
        #region Reviews
        Task<bool>? DeleteReview(int reviewId);
        Task<List<ReviewDto>> GetRecentReviewsAsync(int count, int days = 100);

        #endregion

        #region Payments
        Task<Dictionary<string, int>> GetSalesStatusDistribution();
        Task<decimal> GetTotalRevenue();
        Task<decimal> TotalSalesThisMonth();
        Task<decimal> GrowthRate();
        #endregion



    }
}
