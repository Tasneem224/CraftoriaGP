using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.Categories;
using DomainLayer.Models.Identity;
using DomainLayer.Models.Items;
using DomainLayer.Models.RawMaterials;
using Microsoft.AspNetCore.Identity;
using ServiceAbstraction;
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

namespace Service
{
    public class AdminPanelService(ICloudinaryService _cloudinary, IUnitOfWork _unitOfWork, UserManager<ApplicationUser> _userManager) : IAdminPanelService
    {
        #region Products
        public async Task<ReturnAdminCategoriesDto> AddProductCategoryAsync(AddAdminCategoryDto category)
        {
            string portfolioPath = null;
            portfolioPath = await _cloudinary.UploadAsync(category.image);
            if (category.image is null)
            {
                throw new InvalidOperationExceptionCustome(new List<string> { "Category require image" });
            }

            var cat = new ProductCategory
            {

                NameAr = category.NameAr,
                NameEn = category.NameEn,
                image = portfolioPath,
                IsVisible = true

            };

            await _unitOfWork.AdminPanel.AddProductCategoryAsync(new ProductCategory
            {
                NameAr = cat.NameAr,
                NameEn = cat.NameEn,
                image = cat.image,
                IsVisible = cat.IsVisible
            });
            await _unitOfWork.SaveChangesAsync();
            return new ReturnAdminCategoriesDto
            {
                Id = cat.Id,
                Name=IsArabic() ? cat.NameAr : cat.NameEn,
                image = cat.image,
                IsVisible = cat.IsVisible

            };

        }
        public async Task<bool> DeleteProductCategory(int id)
        {
            await _unitOfWork.AdminPanel.DeleteProductCategory(id);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }
        public async Task<List<ReturnProductDto>> GetAllProductsPagination(int pageNumber, int pageSize)
        {
            var isArabic = IsArabic();
            var products = await _unitOfWork.AdminPanel.GetAllProductsPangination(pageNumber, pageSize);
            return products.Where(p => p.IsVisible == true)
               .Select(p => new ReturnProductDto
               {
                   Id = p.Id,
                   Name = isArabic ? p.NameAr : p.NameEn,
                   Description = isArabic ? p.DescriptionAr : p.DescriptionEn,
                   Price = p.Price,
                   Quantity = p.Quantity,
                   IsVisible = p.IsVisible,
                   CategoryId = p.CategoryId,
                   CategoryName = p.Category.NameEn,
                   ImageUrl = p.ImageUrl
               }).ToList();

        }
        public async Task<int> GetCountNewProductsLast30DaysAgo()
        {
            return await _unitOfWork.AdminPanel.GetCountNewProductsLast30DaysAgo();
        }
        public async Task<int> GetCountProductsAsync()
        {
            return await _unitOfWork.AdminPanel.GetCountProducts();
        }
        public async Task<List<ReturnProductDto>> GetNewProductsLast30DaysAgo(int pageNumber, int pageSize)
        {
            var products = await _unitOfWork.AdminPanel.GetNewProductsLast30DaysAgo(pageNumber, pageSize);
            var isArabic = IsArabic();
            return products.Where(p => p.IsVisible == true)
                .Select(p => new ReturnProductDto
                {
                    Id = p.Id,
                    Name = isArabic ? p.NameAr : p.NameEn,
                    Description = isArabic ? p.DescriptionAr : p.DescriptionEn,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    IsVisible = p.IsVisible,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.NameEn,
                    ImageUrl = p.ImageUrl,
                    SellerName = p.Seller.FirstName + " " + p.Seller.SecondName,
                    SellerId = p.SellerId,

                }).ToList();
        }
        public async Task<bool> ToggleProductVisibility(int id, bool isVisible)
        {
            await _unitOfWork.AdminPanel.ToggleProductVisibility(id, isVisible);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }
        public async Task<ReturnAdminCategoriesDto> UpdateProductCategoryAsync(UpdateAdminCategoryDto category)
        {
            var exist = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(category.Id);
            if (exist is null) throw new ItemNotFound(category.Id.ToString());

            if (category.NameAr != null) exist.NameAr = category.NameAr;
            if (category.NameEn != null) exist.NameEn = category.NameEn;
            if (category.IsVisible != null) exist.IsVisible = category.IsVisible ?? true;

            if (category.image != null)
                exist.ImageUrl = await _cloudinary.UploadAsync(category.image);

            await _unitOfWork.SaveChangesAsync();

            return new ReturnAdminCategoriesDto
            {
                Id = exist.Id,
                Name = IsArabic() ? exist.NameAr : exist.NameEn,
                image = exist.ImageUrl,
                IsVisible = exist.IsVisible
            };

        }

        #endregion

     
        #region Materials
        public async Task<ReturnAdminCategoriesDto> AddRawMaterialCategoryAsync(AddAdminCategoryDto category)

        {
            var imagePath = "";
            if (category.image is null)
            {
                throw new InvalidOperationExceptionCustome(new List<string> { "Category requires Image." });
            }
            imagePath = await _cloudinary.UploadAsync(category.image);
            var material = new Raw_Category_Material
            {

                NameAr = category.NameAr,
                NameEn = category.NameEn,
                image = imagePath,
                IsVisible = true
            };
            await _unitOfWork.AdminPanel.AddRawMaterialCategoryAsync(material);
            await _unitOfWork.SaveChangesAsync();
            return new ReturnAdminCategoriesDto
            {
                Id = material.Id,
                Name = IsArabic() ? material.NameAr : material.NameEn,
                image = material.image,
                IsVisible = material.IsVisible
            };
        }
        public async Task<int> GetCountMaterialsAsync()
        {
            return await _unitOfWork.AdminPanel.GetCountMaterialsAsync();

        }
        public async Task<int> GetCountNewMaterialsLast30Days()
        {
            return await _unitOfWork.AdminPanel.GetCountNewProductsLast30DaysAgo();
        }
        public async Task<List<ReturnProductDto>> GetNewMaterialsLast30DaysAgo(int pageNumber, int pageSize)
        {
            var isArabic = IsArabic();
            var materilas = await _unitOfWork.AdminPanel.GetNewMaterialssLast30DaysAgo(pageNumber, pageSize);
            return materilas.Where(p => p.IsVisible == true)
                .Select(p => new ReturnProductDto
                {
                    Id = p.Id,
                    Name = isArabic ? p.NameAr : p.NameEn,
                    Description = isArabic ? p.DescriptionAr : p.DescriptionEn,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    IsVisible = p.IsVisible,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.NameEn,
                    ImageUrl = p.ImageUrl,
                    SellerName = p.supplier.FirstName + " " + p.supplier.SecondName,
                    SellerId = p.supplierId,
                }).ToList();

        }
        public async Task<bool> DeleteRawMaterialCategory(int id)
        {
            await _unitOfWork.AdminPanel.DeleteRawMaterialCategory(id);
            return await _unitOfWork.SaveChangesAsync() > 0;

        }
        public async Task<List<ReturnProductDto>> GetAllRawMaterialPagination(int pageNumber, int pageSize)
        {
            var isArabic = IsArabic();
            var products = await _unitOfWork.AdminPanel.GetAllGetRawMaterialPagination(pageNumber, pageSize);
            return products.Where(p => p.IsVisible == true)
               .Select(p => new ReturnProductDto
               {
                   Id = p.Id,
                   Name = isArabic ? p.NameAr : p.NameEn,
                   Description = isArabic ? p.DescriptionAr : p.DescriptionEn,
                   Price = p.Price,
                   Quantity = p.Quantity,
                   IsVisible = p.IsVisible,
                   CategoryId = p.CategoryId,
                   CategoryName = p.Category.NameEn,
                   ImageUrl = p.ImageUrl
               }).ToList();
        }
        public async Task<bool> ToggleRawMaterialVisibility(int id, bool isVisible)
        {
            await _unitOfWork.AdminPanel.ToggleRawMaterialVisibility(id, isVisible);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }
        
        public async Task<ReturnAdminCategoriesDto> UpdateRawMaterialCategory(UpdateAdminCategoryDto category)
        {
            var exist = await _unitOfWork.GetRepository<RawMaterial, int>().GetByIdAsync(category.Id);
            if (exist is null) throw new ItemNotFound(category.Id.ToString());

            if (category.NameAr != null) exist.NameAr = category.NameAr;
            if (category.NameEn != null) exist.NameEn = category.NameEn;
            if (category.IsVisible != null) exist.IsVisible = category.IsVisible ?? true;

            if (category.image != null)
                exist.ImageUrl = await _cloudinary.UploadAsync(category.image);

            await _unitOfWork.SaveChangesAsync();

            return new ReturnAdminCategoriesDto
            {
                Id = exist.Id,
                Name= IsArabic() ? exist.NameAr : exist.NameEn,
                image = exist.ImageUrl,
                IsVisible = exist.IsVisible
            };
        }
        #endregion

        #region Users
        public async Task<bool> DeleteUser(string id)
        {
            var exist = await _userManager.FindByIdAsync(id);
            if (exist is null)
                throw new UserIdNotFoundException();
            await _unitOfWork.AdminPanel.DeleteUser(id);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }
        public Task<int> GetTotalUsersCount()
        {
            return _unitOfWork.AdminPanel.GetTotalUsersCount();
        }
        public async Task<List<ReturnAccountDto>> GetAllUsersAsync(int pageNumber, int pageSize)
        {
            var users = await _unitOfWork.AdminPanel.GetAllUsersAsync(pageNumber, pageSize);
            return users.Select(users => new ReturnAccountDto
            {
                UserId = users.Id,
                FirstName = users.FirstName,
                SecondName = users.SecondName,
                Email = users.Email,
                ProfileImage = users.ProfileImage,
                Bio = users.Bio,
                YearOfExperience = users.YearsOfExperience ?? 0,
                Specialization = users.Specialization ?? string.Empty,
                Role = _userManager.GetRolesAsync(users).Result.FirstOrDefault() ?? string.Empty,
            }).ToList();

        }
        public async Task<int> GetCountNewUsersLast30DaysAsync()
        {
            return await _unitOfWork.AdminPanel.GetCountNewUsersLast30DaysAsync();
        }
        public async Task<ReturnAccountDto> GetUserByIdAsync(string id)
        {
            var exist = await _userManager.FindByIdAsync(id);
            if (exist is null)
                throw new UserIdNotFoundException();
            return new ReturnAccountDto
            {
                UserId = exist.Id,
                FirstName = exist.FirstName,
                SecondName = exist.SecondName,
                Email = exist.Email,
                ProfileImage = exist.ProfileImage,
                Bio = exist.Bio,
                YearOfExperience = exist.YearsOfExperience ?? 0,
                Specialization = exist.Specialization ?? string.Empty,
                Role = _userManager.GetRolesAsync(exist).Result.FirstOrDefault() ?? string.Empty,

            };
        }
        public async Task<bool> ToggleUserBlockStatus(string userId, bool isBlocked)
        {
            var exist = await _userManager.FindByIdAsync(userId);
            if (exist is null)
                throw new UserIdNotFoundException();
            await _unitOfWork.AdminPanel.ToggleUserBlockStatus(userId, isBlocked);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }
        public async Task<bool> UpgradeUsersInRole(string id)
        {
            var exist = await _userManager.FindByIdAsync(id);
            if (exist is null)
                throw new UserIdNotFoundException();
            await _unitOfWork.AdminPanel.UpgradeUsersInRole(id);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }
        #endregion

        #region Orders
        public async Task<List<OrderToReturnDto>> GetAllOrdersAsync()
        {
            var orders = await _unitOfWork.AdminPanel.GetAllOrdersAsync();

            return orders.Select(o => new OrderToReturnDto
            {
                Id = o.Id,
                DeliveryMethod = o.DeliveryMethod.ToString(),
                OrderDate = o.OrderDate,
                Subtotal = o.Subtotal,
                OrderItems = o.OrderItems.Select(i => new OrderItemDto
                {
                    ProductId = i.Item.ItemId,
                    PictureUrl = i.Item.ItemPictureUrl,
                    ProductName = i.Item.ItemName,
                    Price = i.Price,
                    Quantity = i.Quantity
                }).ToList()
            }).ToList();
        }
        public async Task<OrderToReturnDto> GetOrderDetails(Guid orderId)
        {
            var order = await _unitOfWork.AdminPanel.GetOrderDetails(orderId);
            return new OrderToReturnDto
            {
                DeliveryMethod = order.DeliveryMethod.ToString(),
                OrderDate = order.OrderDate,
                OrderItems = order.OrderItems.Select(i => new OrderItemDto
                {
                    ProductId = i.Item.ItemId,
                    PictureUrl = i.Item.ItemPictureUrl,
                    ProductName = i.Item.ItemName,
                    Price = i.Price,
                    Quantity = i.Quantity
                }).ToList(),
                Subtotal = order.Subtotal,
                Total = order.GetTotal(),
                UserEmail = order.UserEmail,
                OrderPaymentStatus = order.orderPaymentStatus.ToString(),

            };

        }
        public async Task<int> GetPendingOrdersCountAsync()
        {
            return await _unitOfWork.AdminPanel.GetPendingOrdersCountAsync();

        }
        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, string status)
        {
            await _unitOfWork.AdminPanel.UpdateOrderStatusAsync(orderId, status);
            return await _unitOfWork.SaveChangesAsync() > 0;

        }
        #endregion

        #region Reviews
        public async Task<List<ReviewDto>> GetRecentReviewsAsync(int count, int days = 100)
        {
            var reviews = await _unitOfWork.AdminPanel.GetRecentReviewsAsync(count, days);
            return reviews.Select(r => new ReviewDto
            {
                InteractionId = r.Id,
                ReviewerName = r.User.FirstName + " " + r.User.SecondName,
                UserImage = r.User.ProfileImage,
                ReviewComment = r.Review,
                Rating = r.Rating??0,
                CreatedAt = r.CreatedAt,
                ItemId = r.ProductId ?? r.RawMaterialId ?? 0,
                ItemImage = r.ProductId != null ? r.Product.ImageUrl : r.RawMaterial.ImageUrl,
                ItemName = r.ProductId != null ? (IsArabic() ? r.Product.NameAr : r.Product.NameEn) : (IsArabic() ? r.RawMaterial.NameAr : r.RawMaterial.NameEn),
                ReviewerId = r.UserId

            }).ToList();
        }
        public async Task<bool>? DeleteReview(int reviewId)
        {
            await _unitOfWork.AdminPanel.DeleteReview(reviewId)!;
            return await _unitOfWork.SaveChangesAsync() > 0;
        }
        #endregion



        public bool IsArabic() => Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");
        public Task<decimal> TotalSalesThisMonth()
        {
            throw new NotImplementedException();
        }
        public Task<decimal> GrowthRate()
        {
            throw new NotImplementedException();
        }
        public Task<Dictionary<string, int>> GetSalesStatusDistribution()
        {
            throw new NotImplementedException();
        }
        public Task<List<object>> GetTopSellingProducts(int count)
        {
            throw new NotImplementedException();
        }
        public Task<decimal> GetTotalRevenue()
        {
            throw new NotImplementedException();
        }

    }
}
