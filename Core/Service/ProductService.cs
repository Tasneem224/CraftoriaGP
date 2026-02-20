using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.Categories;
using DomainLayer.Models.Identity;
using DomainLayer.Models.Items;
using FuzzySharp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ServiceAbstraction;
using Shared.ErrorModels;
using Shared.IdentityModule;
using Shared.ProductModule;
using System.Globalization;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Service
{
    public class ProductService(ITranslationService _translationService,IHttpContextAccessor _httpContextAccessor,UserManager<ApplicationUser> _userManager, ICloudinaryService _cloudinary, IUnitOfWork _unitOfWork) : IProductService
    {

        public async Task<IEnumerable<ReturnProductDto>> GetAllProductsAsync()
        {
            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");

            var productRepo =  _unitOfWork.GetRepository<Product, int>();
                
            var products = await productRepo.GetAllAsync();

            var categoryRepo = _unitOfWork.GetRepository<ProductCategory, int>();
            var categories = await categoryRepo.GetAllAsync();

            var categoriesDict = categories.ToDictionary(c => c.Id, c => c.Name);

            return ReturnListDto(isArabic, products, categoriesDict);
        }
        public async Task<ReturnProductDto> GetProductByIdAsync(int id)
        {
            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");

            var productRepo = _unitOfWork.GetRepository<Product, int>();
            var product = await productRepo.GetByIdAsync(id);

            if (product == null) throw new ItemNotFound("this product is not found");

            var categoryRepo = _unitOfWork.GetRepository<ProductCategory, int>();
            var category = await categoryRepo.GetByIdAsync(product.CategoryId);
            var user =await _userManager.FindByIdAsync(product.SellerId);
            var userName = user!.FirstName + " " + user.SecondName;
            return ReturnDto(isArabic, product, category);

        }
        public async Task<ReturnProductDto> AddProductAsync(CreateProductDto dto)
        {
            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");

           var sellerId= AuthFun(isArabic);

            string imageUrl;
            if (dto.ImageFile != null)
                    imageUrl = await _cloudinary.UploadAsync(dto.ImageFile);
            

            else
            {
                throw new Exception("you should upload image");
            }
            string DescAr;
            string DescEn;
            if (isArabic)
            {
                DescAr = dto.Description!;
                DescEn = await _translationService.TranslateAsync(dto.Description!, "en");
            }
            else
            {
                DescEn = dto.Description!;
                DescAr = await _translationService.TranslateAsync(dto.Description!, "ar");
            }

            var product = new Product
            {
                NameEn = dto.NameEn,
                NameAr = dto.NameAr,
                Price = dto.Price,
                Quantity = dto.Quantity ?? 0,
                DescriptionAr = DescAr,
                DescriptionEn = DescEn,
                CategoryId = dto.CategoryId,
                SellerId = sellerId!,
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
        public async Task<ReturnProductDto> UpdateProductAsync(int id, UpdateProductDto dataFromRequest)
        {

            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");

            AuthFun(isArabic);

            var repo = _unitOfWork.GetRepository<Product, int>();
            var product = await repo.GetByIdAsync(id);

            if (product == null)
            {
                throw new ItemNotFound("this product not found");
            }
            (string DescAr, string DescEn) = await TranslateDescription(dataFromRequest, isArabic);

            product.NameEn = dataFromRequest.NameEn ?? product.NameEn;
            product.NameAr = dataFromRequest.NameAr ?? product.NameAr;

            if (dataFromRequest.Price.HasValue && dataFromRequest.Price > 0)
                product.Price = dataFromRequest.Price.Value;

            if (dataFromRequest.CategoryId.HasValue && dataFromRequest.CategoryId > 0)
                product.CategoryId = dataFromRequest.CategoryId.Value;
            if (!string.IsNullOrWhiteSpace(dataFromRequest.Description))
            {
                (string descAr, string descEn) = await TranslateDescription(dataFromRequest, isArabic);
                product.DescriptionAr = descAr;
                product.DescriptionEn = descEn;
            }

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


            return ReturnDto(isArabic, product, category);
        }
        public async Task<bool> DeleteProductAsync(int id)
        {
            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");

            AuthFun(isArabic);
            var repo = _unitOfWork.GetRepository<Product, int>();
            var product = await repo.GetByIdAsync(id);
            if (product == null)
            {
                throw new ItemNotFound("this item is already not found");
            }
            
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
        public async Task<IEnumerable<ReturnProductDto>> GetAllProductsOfSpecifiUserAsync(string id)
        {
            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");

            var checkUser = await _userManager.FindByIdAsync(id);
            if (checkUser is null)
            {
                throw new UserNotFoundException("this user is not found");

            }
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(user!);
            foreach (var role in roles)
            {
                if (role != RoleType.Beginner.ToString() && role != RoleType.Expert.ToString())
                {
                    throw new InvalidOperationException("role is not valid to do this operation");
                }
            }
            var repo = _unitOfWork.GetRepository<Product, int>();
            var query = await repo.GetAllAsync();
            var products = query.Where(p => p.SellerId == id);

            var categoryRepo = _unitOfWork.GetRepository<ProductCategory, int>();
            var categories = await categoryRepo.GetAllAsync();

            var categoriesDict = categories.ToDictionary(c => c.Id, c => c.Name);
            return ReturnListDto(isArabic, products, categoriesDict);

        }
        public async Task<int> GetProductsCountByUserIdAsync(string userId)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new UserNotFoundException("this user is not found");
                }

                var repo = _unitOfWork.GetRepository<Product, int>();
                var query = await repo.GetAllAsync();

                return query.Count(p => p.SellerId == userId);
            }
        public async Task<IEnumerable<ReturnProductsOfCategory>> GetAllProductsOfSpecificCategory(int id)
        {
            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");

            var repo = _unitOfWork.GetRepository<Product, int>();
            var query = await repo.GetAllAsync();
            return query
                    .Where(p => p.CategoryId == id)
                    .Select(p => new ReturnProductsOfCategory
                    {
                        Id = p.Id,
                        Image = p.ImageUrl ?? "",
                        Name = isArabic ? p.NameAr : p.NameEn,
                        Description = isArabic ? p.DescriptionAr! : p.DescriptionEn!,
                        Price = p.Price
                    }).ToList();
        }

        private async Task<(string DescAr, string DescEn)> TranslateDescription(UpdateProductDto dataFromRequest, bool isArabic)
        {
            string DescAr;
            string DescEn;
            if (isArabic)
            {
                DescAr = dataFromRequest.Description!;
                DescEn = await _translationService.TranslateAsync(dataFromRequest!.Description!, "en");
            }
            else
            {
                DescEn = dataFromRequest.Description!;
                DescAr = await _translationService.TranslateAsync(dataFromRequest?.Description!, "ar");
            }

            return (DescAr, DescEn);
        }
        private static ReturnProductDto ReturnDto(bool isArabic, Product product, ProductCategory category)
        {
            return new ReturnProductDto
            {
                Id = product.Id,
                Name = isArabic ? product.NameAr : product.NameEn,
                Price = product.Price,
                Quantity = product.Quantity ?? 0,
                Description = isArabic?product.DescriptionAr: product.DescriptionEn,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                SellerId = product.SellerId,
                CategoryName = category != null ? category.Name : "",
                SellerName = product.Seller?.FirstName+" "+ product.Seller?.SecondName ,
            };
        }
        private string? AuthFun(bool isArabic)
        {
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
            return sellerId;
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
                throw new Exception();
            }
        }
        private static IEnumerable<ReturnProductDto> ReturnListDto(bool isArabic, IEnumerable<Product> products, Dictionary<int, string> categoriesDict)
        {
            return products.Select(p => new ReturnProductDto
            {
                Id = p.Id,
                Name = isArabic ? p.NameAr : p.NameEn,
                Price = p.Price,
                Quantity = p.Quantity ?? 0,
                Description = isArabic ? p.DescriptionAr : p.DescriptionEn,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                SellerId = p.SellerId,
                CategoryName = categoriesDict.ContainsKey(p.CategoryId)
                               ? categoriesDict[p.CategoryId]
                               : "Unknown"
            }).ToList();
        }

    }
}
