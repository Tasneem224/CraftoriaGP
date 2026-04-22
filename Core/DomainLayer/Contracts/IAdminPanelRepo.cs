using DomainLayer.Models.Categories;
using DomainLayer.Models.Identity;
using DomainLayer.Models.Interaction;
using DomainLayer.Models.Items;
using DomainLayer.Models.Order;
using DomainLayer.Models.RawMaterials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Contracts
{

    public interface IAdminPanelRepo
    {
        #region Products
        Task<List<Product>> GetNewProductsLast30DaysAgo(int pageNumber, int pageSize);
        Task<List<Product>> GetAllProductsPangination(int pageNumber, int pageSize);
        Task<int> GetCountNewProductsLast30DaysAgo();
        Task<ProductCategory> AddProductCategoryAsync(ProductCategory category);
        Task<bool> DeleteProductCategory(int id );
        Task<ProductCategory> UpdateProductCategoryAsync(int id, ProductCategory category);
        Task<bool> ToggleProductVisibility(int id, bool isVisible);
        Task<int> GetCountProducts();

        #endregion

        #region Raw Materials
        Task<int> GetCountMaterialsAsync();

        Task<int> GetCountNewMaterialssLast30DaysAgo();
        Task<List<RawMaterial>> GetNewMaterialssLast30DaysAgo(int pageNumber, int pageSize);
        Task<List<RawMaterial>> GetAllGetRawMaterialPagination(int pageNumber, int pageSize);
        Task<Raw_Category_Material> AddRawMaterialCategoryAsync(Raw_Category_Material category);
        Task<bool> DeleteRawMaterialCategory(int id);
        Task<Raw_Category_Material> UpdateRawMaterialCategory(int id, Raw_Category_Material category);
        Task<bool> ToggleRawMaterialVisibility(int id, bool isVisible);
        #endregion


        #region users
        Task<int> GetTotalUsersCount();
        Task<List<ApplicationUser>> GetAllUsersAsync(int pageNumber, int pageSize);
        Task<List<ApplicationUser>> GetNewUsersLast30Days(int pageNumber, int pageSize);
        Task<int> GetCountNewUsersLast30DaysAsync();
        Task<ApplicationUser> GetUserById(string id);
        Task<bool> DeleteUser(string id);
        Task<bool> UpgradeUsersInRole(string id);
        Task<bool> ToggleUserBlockStatus(string userId, bool isBlocked);
        #endregion


        #region sessions
        #endregion

        #region Orders
        Task<List<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderDetails(Guid orderId);
        Task<bool> UpdateOrderStatusAsync(Guid orderId, string status);
        #endregion

        #region Reviews
        Task<bool>? DeleteReview(int reviewId);
        Task<List<UserInteraction>> GetRecentReviewsAsync(int count, int days);

        #endregion
        #region Payments
        Task<decimal> GetTotalRevenue();
        #endregion

        Task<decimal> GrowthRate();
        Task<decimal> TotalSalesThisMonth();
        Task<int> GetPendingOrdersCountAsync();
        Task<Dictionary<string, int>> GetSalesStatusDistribution();
        Task<List<object>> GetTopSellingProducts(int count);
    }

}
