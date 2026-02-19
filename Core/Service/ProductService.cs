using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.Categories;
using DomainLayer.Models.Identity;
using DomainLayer.Models.Items;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ServiceAbstraction;
using Shared.IdentityModule;
using Shared.ProductModule;
using System.Security.Claims;

namespace Service
{
    public class ProductService(IHttpContextAccessor _httpContextAccessor,UserManager<ApplicationUser> _userManager, ICloudinaryService _cloudinary, IUnitOfWork _unitOfWork) : IProductService
    {
        public async Task<IEnumerable<ReturnProductDto>> GetAllProductsAsync()
        {
            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");

            var productRepo = _unitOfWork.GetRepository<Product, int>();
            var products = await productRepo.GetAllAsync();

            var categoryRepo = _unitOfWork.GetRepository<ProductCategory, int>();
            var categories = await categoryRepo.GetAllAsync();

            var categoriesDict = categories.ToDictionary(c => c.Id, c => c.Name);

            return products.Select(p => new ReturnProductDto
            {
                Id = p.Id,
                Name = isArabic ? p.NameAr : p.NameEn,
                Price = p.Price,
                Quantity = p.Quantity ?? 0,
                Description = p.DescriptionEn,
                ImageUrl = p.ImageUrl, 
                CategoryId = p.CategoryId,
                SellerId = p.SellerId,
                CategoryName = categoriesDict.ContainsKey(p.CategoryId)
                               ? categoriesDict[p.CategoryId]
                               : "Unknown"
            }).ToList();
        }

        public async Task<ReturnProductDto> GetProductByIdAsync(int id)
        {
            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");

            var productRepo = _unitOfWork.GetRepository<Product, int>();
            var product = await productRepo.GetByIdAsync(id);

            if (product == null) return null;

            var categoryRepo = _unitOfWork.GetRepository<ProductCategory, int>();
            var category = await categoryRepo.GetByIdAsync(product.CategoryId);
            var user = _userManager.FindByIdAsync(product.SellerId).Result;
            var userName = user.FirstName + " " + user.SecondName;
            return ReturnDto(isArabic, product, category);

        }
        public async Task<ReturnProductDto> AddProductAsync(CreateProductDto dto)
        {
            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");

            var sellerId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(sellerId))
                throw new UnauthorizedAccessException(isArabic ? "يجب تسجيل الدخول أولاً" : "Unauthorized: Please login");

            var user = _httpContextAccessor?.HttpContext?.User;

            bool isAuthorizedRole = user!.IsInRole(RoleType.Beginner.ToString()) ||
                                     user.IsInRole(RoleType.Expert.ToString());

            if (!isAuthorizedRole)
            {
                throw new InvalidOperationException(isArabic
                    ? "غير مسموح لك بإضافة منتجات، يجب أن تكون مبتدئ أو خبير"
                    : "Role is not valid to do this operation");
            }


            string imageUrl = null;
            if (dto.ImageFile != null)
            {
                try
                {
                    imageUrl = await _cloudinary.UploadAsync(dto.ImageFile);

                }
                catch (Exception)
                {
                    throw;
                }
            }

            else
            {
                throw new Exception("you should upload image");
            }


            var product = new Product
            {
                NameEn = dto.NameEn,
                NameAr = dto.NameAr,
                Price = dto.Price,
                Quantity = dto.Quantity ?? 0,
                DescriptionEn = dto.Description,
                CategoryId = dto.CategoryId,
                SellerId = sellerId,
                ImageUrl = imageUrl
            };
            if (dto.Tags != null && dto.Tags.Any())
            {
                var tagRepo = _unitOfWork.GetRepository<Tag, int>();
                var existingTags = await tagRepo.GetAllAsync();

                foreach (var tagName in dto.Tags)
                {
                    var cleanTagName = tagName.Trim();

                    var tag = existingTags.FirstOrDefault(t => t.Name.Equals(cleanTagName, StringComparison.OrdinalIgnoreCase));

                    if (tag != null)product.tags.Add(tag);
                    else
                    {
                        var newTag = new Tag { Name = cleanTagName };
                        product.tags.Add(newTag);
                    }
                }
            }
            await _unitOfWork.GetRepository<Product, int>().AddAsync(product);
            await _unitOfWork.SaveChanges();

            var categoryRepo = _unitOfWork.GetRepository<ProductCategory, int>();
            var category = await categoryRepo.GetByIdAsync(dto.CategoryId);

            return ReturnDto(isArabic, product, category);
        }

        private static ReturnProductDto ReturnDto(bool isArabic, Product product, ProductCategory category)
        {
            return new ReturnProductDto
            {
                Id = product.Id,
                Name = isArabic ? product.NameAr : product.NameEn,
                Price = product.Price,
                Quantity = product.Quantity ?? 0,
                Description = product.DescriptionEn,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                SellerId = product.SellerId,
                CategoryName = category != null ? category.Name : ""
            };
        }

        public async Task<ReturnProductDto> UpdateProductAsync(int id, UpdateProductDto dataFromRequest)
        {
            var repo = _unitOfWork.GetRepository<Product, int>();
            var product = await repo.GetByIdAsync(id);

            if (product == null)
            {
                throw new ItemNotFound("this product not found");
            }

            product.NameEn = dataFromRequest.NameEn==null?product.NameEn : dataFromRequest.NameEn;
            product.Price = dataFromRequest.Price==0.0m?product.Price: dataFromRequest.Price;
            product.DescriptionEn = dataFromRequest.Description==null?product.DescriptionEn: dataFromRequest.Description;
            product.CategoryId = dataFromRequest.CategoryId==0?product.CategoryId: dataFromRequest.CategoryId;
            product.Quantity = dataFromRequest.Quantity==null?product.Quantity: dataFromRequest.Quantity;
            //if (dataFromRequest.Quantity.HasValue) product.Quantity = dataFromRequest.Quantity.Value;

            
            if (dataFromRequest.ImageFile != null)
            {
                
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    string publicId = GetPublicIdFromUrl(product.ImageUrl);
                    if (!string.IsNullOrEmpty(publicId))
                    {
                        _cloudinary.DeleteAsync(publicId);
                    }
                }

                product.ImageUrl = await _cloudinary.UploadAsync(dataFromRequest.ImageFile);
            }

            repo.Update(product);
            await _unitOfWork.SaveChanges();

            var categoryRepo = _unitOfWork.GetRepository<ProductCategory, int>();
            var category = await categoryRepo.GetByIdAsync(product.CategoryId);

            return new ReturnProductDto
            {
                Id = product.Id,
                Name = product.NameEn,
                Price = product.Price,
                Quantity = product.Quantity ?? 0,
                Description = product.DescriptionEn,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                SellerId = product.SellerId,
                CategoryName = category != null ? category.Name : "Unknown"
            };
        }
        [Authorize]

        public async Task<bool> DeleteProductAsync(int id)
        {
            var repo = _unitOfWork.GetRepository<Product, int>();
            var product = await repo.GetByIdAsync(id);
            if (product == null)
            {
                throw new ItemNotFound("this item is already not found");
            }
            

            // Delete image from Cloudinary before deleting product
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                string publicId = GetPublicIdFromUrl(product.ImageUrl);
                if (!string.IsNullOrEmpty(publicId))
                {
                    _cloudinary.DeleteAsync(publicId);
                }
            }

            repo.Remove(product);
            await _unitOfWork.SaveChanges();
            return true;
        }

 
        private string GetPublicIdFromUrl(string url)
        {
            try
            {
                var uri = new Uri(url);
                var segments = uri.Segments; 
                var imageName = Path.GetFileNameWithoutExtension(url);
                return $"images/{imageName}";
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<ReturnProductDto>> GetAllProductsOfSpecifiUserAsync(string id)
        {
            var checkUser = await _userManager.FindByIdAsync(id);
            if (checkUser is null)
            {
                throw new UserNotFoundException("this user is not found");

            }
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                if (role != RoleType.Beginner.ToString() && role != RoleType.Expert.ToString())
                {
                    throw new InvalidOperationException("role is not valid to do this operation");
                }
            }
            var repo = _unitOfWork.GetRepository<Product, int>();
            var query =await repo.GetAllAsync();
            var products = query.Where(p => p.SellerId == id);

            var categoryRepo = _unitOfWork.GetRepository<ProductCategory, int>();
            var categories = await categoryRepo.GetAllAsync();

            var categoriesDict = categories.ToDictionary(c => c.Id, c => c.Name);

            return products.Select(p => new ReturnProductDto
            {
                Id = p.Id,
                Name = p.NameEn,
                Price = p.Price,
                Quantity = p.Quantity ?? 0,
                Description = p.DescriptionEn,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                SellerId = p.SellerId,
                CategoryName = categoriesDict.ContainsKey(p.CategoryId)
                               ? categoriesDict[p.CategoryId]
                               : "Unknown"
            }).ToList();


        }

        public async Task<int> GetProductsCountByUserIdAsync(string userId)
        {
            // 1. التأكد من وجود المستخدم (اختياري حسب رغبتك)
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new UserNotFoundException("this user is not found");
            }

            // 2. الحصول على الـ Repository وعمل Count للمنتجات الخاصة بهذا المستخدم
            var repo = _unitOfWork.GetRepository<Product, int>();
            var query = await repo.GetAllAsync();

            return query.Count(p => p.SellerId == userId);
        }
    }
}
