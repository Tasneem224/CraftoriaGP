using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.Categories;
using DomainLayer.Models.Identity;
using DomainLayer.Models.Interaction;
using DomainLayer.Models.Items;
using DomainLayer.Models.Order;
using DomainLayer.Models.RawMaterials;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistance.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class AdminPanelRepo(StoreDbContext _context, UserManager<ApplicationUser> _userManager) : IAdminPanelRepo
    {
        #region Products
        public async Task<ProductCategory> AddProductCategory(ProductCategory category)
        {
            var product = await _context.ProductCategories.AddAsync(category);
            return product.Entity;
        }
        public async Task<List<Product>> GetAllProductsPangination(int pageNumber, int pageSize)
            => await _context.Products
            .Include(p=>p.Category)
            .Skip((pageNumber - 1) * pageSize)
              .Take(pageSize)
              .ToListAsync();
        public async Task<int> GetCountNewProductsLast30DaysAgo()
        {
            var last30Days = DateTime.UtcNow.AddDays(-30);
            return await _context.Products.Where(o => o.CreatedAt >= last30Days).CountAsync();
        }
        public async Task<List<Product>> GetNewProductsLast30DaysAgo(int pageNumber, int pageSize)
        {
            var last30Days = DateTime.UtcNow.AddDays(-30);
            return await _context.Products.Where(o => o.CreatedAt >= last30Days)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync() ;
        }
        public async Task<bool> DeleteProductCategory(ProductCategory category)
        {
            var exist = await _context.ProductCategories.FindAsync(category);
            if (exist is null) return false;
            _context.ProductCategories.Remove(exist);
            return true;
        }
        public async Task<Product> UpdateProduct(int id, Product product)
        {
            var exist = await _context.Products.FindAsync(id);
            if (exist is null) return null!;
            _context.Entry(exist).CurrentValues.SetValues(product);
            return exist;
        }
        public async Task<ProductCategory> UpdateProductCategory(int id, ProductCategory category)
        {
            var exist = await _context.ProductCategories.FindAsync(id);
            if (exist is null) return null!;
            _context.Entry(exist).CurrentValues.SetValues(category);
            return exist;
        }
        public async Task<bool> ToggleProductVisibility(int id, bool isVisible)
        {
            var exist = await _context.Products.FindAsync(id);
            if (exist is null) return false;
            exist.IsVisible = isVisible;
            return true;
        }
        #endregion

        #region Raw Materials

        public async Task<Raw_Category_Material> AddRawMaterialCategory(Raw_Category_Material category)
        {
            var material = await _context.RawMaterialCategories.AddAsync(category);
            return material.Entity;
        }
        public async Task<bool> DeleteRawMaterialCategory(Raw_Category_Material category)
        {
            var exist = await _context.RawMaterialCategories.FindAsync(category);
            if (exist is null) return false;
            _context.RawMaterialCategories.Remove(exist);
            return true;

        }
        public async Task<List<RawMaterial>> GetAllGetRawMaterialPagination(int pageNumber, int pageSize)
             => await _context.RawMaterials
                .Include(p=>p.Category)
                 .Skip((pageNumber - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();
        public async Task<int> GetCountNewMaterialssLast30DaysAgo()
        {
            var last30Days = DateTime.UtcNow.AddDays(-30); // اتحسب مرة واحدة بس

            return await _context.RawMaterials.Where(m => m.CreatedAt >= last30Days).CountAsync();


            return await _context.RawMaterials.Where(m => m.CreatedAt >= DateTime.UtcNow.AddDays(-30)).CountAsync();
            // ⚠️ بيحسب التاريخ لكل Row في الداتابيز

        }
        public async Task<List<RawMaterial>> GetNewMaterialssLast30DaysAgo(int pageNumber, int pageSize)
        {
            var last30Days = DateTime.UtcNow.AddDays(-30);
            return await _context.RawMaterials.Where(o => o.CreatedAt >= last30Days)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<RawMaterial> UpdateRawMaterial(int id, RawMaterial material)
        {
            var exist = await _context.RawMaterials.FindAsync(id);
            if (exist is null) return null!;
            _context.Entry(exist).CurrentValues.SetValues(material);
            return exist;
        }
        public async Task<Raw_Category_Material> UpdateRawMaterialCategory(int id, Raw_Category_Material category)
        {
            var exist = await _context.RawMaterialCategories.FindAsync(id);
            if (exist is null) return null!;
            _context.Entry(exist).CurrentValues.SetValues(category);
            return exist;
        }
        public async Task<bool> ToggleRawMaterialVisibility(int id, bool isVisible)
        {
            var exist = await _context.RawMaterials.FindAsync(id);
            if (exist is null) return false;
            exist.IsVisible = isVisible;
            return true;
        }
        #endregion

        #region Users
        public async Task<List<ApplicationUser>> GetAllUsers(int pageNumber, int pageSize)
            => await _userManager.Users
            .Skip((pageNumber - 1) * pageSize)
              .Take(pageSize)
               .ToListAsync();
        public Task<int> GetCountNewUsersLast30Days()
        {
            var last30Days = DateTime.UtcNow.AddDays(-30);
            return _userManager.Users.Where(u => u.CreatedAt >= last30Days).CountAsync();
        }
        public async Task<ApplicationUser> GetUserById(string id)
        {
            var exist = await _userManager.FindByIdAsync(id);
            if (exist == null) throw new UserIdNotFoundException();
            return exist;
        }
        public async Task<int> GetTotalUsersCount()
        => await _userManager.Users.CountAsync();
        public async Task<bool> DeleteUser(Guid id)
        {
            var exist = _userManager.Users.FirstOrDefault(u => u.Id == id.ToString());
            if (exist is null) return false;
            await _userManager.DeleteAsync(exist);
            return true;
        }
        public async Task<bool> UpgradeUsersInRole(string id)
        {
            var exist = await _userManager.FindByIdAsync(id);
            if (exist is null) return false;
            var currentRoles = await _userManager.GetRolesAsync(exist);

            if (await _userManager.IsInRoleAsync(exist, "Beginner"))
            {
                var removeResult = await _userManager.RemoveFromRoleAsync(exist, "Beginner");
                if (!removeResult.Succeeded) return false;

                var addResult = await _userManager.AddToRoleAsync(exist, "Expert");
                if (!addResult.Succeeded) return false;
            }
            return true;
        }
        public async Task<bool> ToggleUserBlockStatus(string userId, bool isBlocked)
        {
            var exist = await _userManager.FindByIdAsync(userId);
            if (exist is null) return false;
            exist.IsBlocked = isBlocked;
            return true;
        }
        public async Task<List<ApplicationUser>> GetNewUsersLast30Days(int pageNumber, int pageSize)
        {
            var last30Days = DateTime.UtcNow.AddDays(-30);
            return await _userManager.Users.Where(u => u.CreatedAt >= last30Days)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        #endregion

        #region Reviews
        public async Task<bool>? DeleteReview(int reviewId)
        {
            _context.UserInteractions.Remove(_context.UserInteractions.Find(reviewId) ?? throw new Exception("Review not found"));
            return true;
        }
        public async Task<List<UserInteraction>> GetRecentReviews(int count, int days = 100)
        {
            var fromDate = DateTime.UtcNow.AddDays(-days);

            return await _context.UserInteractions
                .Where(r => r.CreatedAt >= fromDate)
                .OrderByDescending(r => r.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        #endregion

        #region Orders
        public async Task<List<Order>> GetAllOrders()
             => await _context.Orders.Include(o => o.OrderItems).ThenInclude(i => i.Item).ToListAsync();
        public async Task<Order> GetOrderDetails(Guid orderId)
          => await _context.Orders.Where(o => o.Id == orderId)
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Item)
                .FirstOrDefaultAsync() ?? throw new Exception("Order not found");
        public async Task<int> GetPendingOrdersCount() =>
            await _context.Orders.Where(o => o.orderStatus == OrderStatus.Pending).CountAsync();
        public async Task<bool> UpdateOrderStatus(Guid orderId, string status)
        {
            var exist = await _context.Orders.FindAsync(orderId);
            if (exist is null) return false;
            if (!Enum.TryParse<OrderStatus>(status, ignoreCase: true, out OrderStatus orderStatus))
                return false;
            exist.orderStatus = orderStatus;
            return true;

        }
        #endregion


        public Task<List<object>> GetTopSellingProducts(int count)
        {
            throw new NotImplementedException();
        }
        public Task<Dictionary<string, int>> GetSalesStatusDistribution()
        {
            throw new NotImplementedException();
        }
        public Task<decimal> GetTotalRevenue()
        {
            throw new NotImplementedException();
        }
        public Task<decimal> GrowthRate()
        {
            throw new NotImplementedException();
        }
        public Task<decimal> TotalSalesThisMonth()
        {
            throw new NotImplementedException();
        }

    }

}
