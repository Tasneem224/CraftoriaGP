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
        Task<int> GetCountNewProductsLast30DaysAgo();
        Task<List<Product>> GetAllProductsPangination(int pageNumber, int pageSize);
        Task<ProductCategory> AddProductCategory(ProductCategory category);
        Task<bool> DeleteProductCategory(ProductCategory category);
        Task<ProductCategory> UpdateProductCategory(int id, ProductCategory category);
        Task<Product> UpdateProduct(int id, Product product);
        Task<bool> ToggleProductVisibility(int id, bool isVisible);
        #endregion

        #region Raw Materials

        Task<int> GetCountNewMaterialssLast30DaysAgo();
        Task<List<RawMaterial>> GetNewMaterialssLast30DaysAgo(int pageNumber, int pageSize);
        Task<List<RawMaterial>> GetAllGetRawMaterialPagination(int pageNumber, int pageSize);
        Task<Raw_Category_Material> AddRawMaterialCategory(Raw_Category_Material category);
        Task<bool> DeleteRawMaterialCategory(Raw_Category_Material category);
        Task<Raw_Category_Material> UpdateRawMaterialCategory(int id, Raw_Category_Material category);
        Task<RawMaterial> UpdateRawMaterial(int id, RawMaterial material);
        Task<bool> ToggleRawMaterialVisibility(int id, bool isVisible);
        #endregion


        #region users
        Task<int> GetTotalUsersCount();
        Task<List<ApplicationUser>> GetAllUsers(int pageNumber, int pageSize);
        Task<List<ApplicationUser>> GetNewUsersLast30Days(int pageNumber, int pageSize);
        Task<int> GetCountNewUsersLast30Days();
        Task<ApplicationUser> GetUserById(string id);
        Task<bool> DeleteUser(Guid id);
        Task<bool> UpgradeUsersInRole(string id);
        Task<bool> ToggleUserBlockStatus(string userId, bool isBlocked);
        #endregion


        #region sessions
        #endregion

        #region Orders
        Task<List<Order>> GetAllOrders();
        Task<Order> GetOrderDetails(Guid orderId);
        Task<bool> UpdateOrderStatus(Guid orderId, string status);
        #endregion

        #region Reviews
        Task<bool>? DeleteReview(int reviewId);
        Task<List<UserInteraction>> GetRecentReviews(int count, int days);

        #endregion
        #region Payments
        Task<decimal> GetTotalRevenue();
        #endregion

        Task<decimal> GrowthRate();
        Task<decimal> TotalSalesThisMonth();
        Task<int> GetPendingOrdersCount();
        Task<Dictionary<string, int>> GetSalesStatusDistribution();
        Task<List<object>> GetTopSellingProducts(int count);
    }

}
