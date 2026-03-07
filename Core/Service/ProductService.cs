using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.Categories;
using DomainLayer.Models.Identity;
using DomainLayer.Models.Items;
using FuzzySharp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceAbstraction;
using Shared.ErrorModels;
using Shared.Extensions;
using Shared.IdentityModule;
using Shared.ProductModule;
using System.Diagnostics;
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

            var categoriesDict = categories.ToDictionary(c => c.Id, c => c);

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
            var sellerId = AuthFun(isArabic);
            if (dto.ImageFile == null) throw new Exception("You should upload an image");

            async Task<string> UploadWithTimer()
            {
                var result = await _cloudinary.UploadAsync(dto.ImageFile);
                return result;
            }

            async Task<string> TranslateWithTimer()
            {
                string targetLang = isArabic ? "en" : "ar";
                var result = await _translationService.TranslateAsync(dto.Description!, targetLang);
                return result;
            }

            var uploadTask = UploadWithTimer();
            var translateTask = TranslateWithTimer();

            await Task.WhenAll(uploadTask, translateTask);

            string imageUrl = uploadTask.Result;
            string translatedText = translateTask.Result;

            string DescAr = isArabic ? dto.Description! : translatedText;
            string DescEn = isArabic ? translatedText : dto.Description!;

            var dbStopwatch = Stopwatch.StartNew();

            var product = new Product
            {
                NameEn = dto.NameEn,
                NameAr = dto.NameAr.NormalizeArabicText() ?? dto.NameAr,
                Price = dto.Price,
                Quantity = dto.Quantity ?? 0,
                DescriptionAr = DescAr.NormalizeArabicText(),
                DescriptionEn = DescEn,
                CategoryId = dto.CategoryId,
                SellerId = sellerId!,
                ImageUrl = imageUrl
            };

            if (dto.Tags != null && dto.Tags.Any())
            {
                var cleanTags = dto.Tags.Select(t => t.Trim()).ToList();
                var tagRepo = _unitOfWork.GetRepository<Tag, int>();

                var existingTags = await tagRepo.GetAllQueryable()
                .AsTracking()
                .Where(t => cleanTags.Contains(t.Name))
                .ToListAsync();
                foreach (var tagName in cleanTags)
                {
                    var tag = existingTags.FirstOrDefault(t => t.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase));

                    if (tag != null)                  
                        product.tags.Add(tag);
                    else
                      product.tags.Add(new Tag { Name = tagName });
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


            var product = await repo.GetAllQueryable()
                                    .Include(p => p.tags)
                                    .Include(p=>p.Category)
                                    .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                throw new ItemNotFound("this product not found");
            }

            await UpdateData(dataFromRequest, isArabic, product);
            _unitOfWork.GetRepository<Product, int>().Update(product); // بيأكد للـ Tracker إن فيه تعديل
            await _unitOfWork.SaveChanges();

            return ReturnDto(isArabic, product, product.Category);
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

            var categoriesDict = categories.ToDictionary(c => c.Id, c => c);
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
        public async Task<List<ReturnProductDto>> SearchProductsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return new List<ReturnProductDto>();

            var isArabic = CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ar";
            var culture = isArabic ? new CultureInfo("ar-EG") : new CultureInfo("en-US");
            var normalizedQuery1 = query.Trim().ToLower();
            var normalizedQuery= normalizedQuery1.NormalizeArabicText();

            var allProducts = await _unitOfWork.GetRepository<Product, int>()
                .GetAllQueryable()
                .Include(p => p.Category)
                .Include(p => p.Seller) 
                .Include(t=>t.tags)
                .ToListAsync();

            var searchResults = allProducts
              .Select(p => {
                  var tagsText = p.tags != null ? string.Join(" ", p.tags.Select(t => t.Name)) : "";
                  var searchableText = $"{p.NameAr} {p.NameEn} {p.DescriptionAr} {p.DescriptionEn} {tagsText}".ToLower().NormalizeArabicText();

                  var score = Fuzz.WeightedRatio(normalizedQuery, searchableText);

                  return new { Product = p, Score = score, SearchableText = searchableText };
              })
              .Where(
                x => x.Score >= 70 || x.SearchableText.Contains(normalizedQuery))
              .OrderByDescending(x => x.Score)
              .Select(
                x => ReturnDto(isArabic, x.Product, x.Product.Category))
              .ToList();

                    return searchResults;
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
        private async Task UpdateData(UpdateProductDto dataFromRequest, bool isArabic, Product product)
        {
            // 1. التحديثات المباشرة
            product.NameEn = dataFromRequest.NameEn ?? product.NameEn;
            if (!string.IsNullOrWhiteSpace(dataFromRequest.NameAr))
                product.NameAr = dataFromRequest.NameAr.NormalizeArabicText()!;

            if (dataFromRequest.Price.HasValue && dataFromRequest.Price > 0)
                product.Price = dataFromRequest.Price.Value;

            if (dataFromRequest.CategoryId.HasValue && dataFromRequest.CategoryId > 0)
                product.CategoryId = dataFromRequest.CategoryId.Value;

            // 2. إدارة المهمات (Tasks) بطريقة صحيحة
            var tasks = new List<Task>();

            // مهمة الوصف والترجمة
            if (!string.IsNullOrWhiteSpace(dataFromRequest.Description))
            {
                // بدل Task.Run، هنادي الميثود مباشرة ونضيف الـ Task للـ List
                tasks.Add(UpdateDescriptionAsync(dataFromRequest, isArabic, product));
            }

            // مهمة الصورة
            if (dataFromRequest.ImageFile != null)
            {
                tasks.Add(UpdateImageAsync(dataFromRequest, product));
            }

            // 3. تحديث الـ Tags (يجب أن يتم هنا لضمان الـ Tracking)
            if (dataFromRequest.Tags != null)
            {
                product.tags.Clear();
                if (dataFromRequest.Tags.Any())
                {
                    var tagRepo = _unitOfWork.GetRepository<Tag, int>();
                    var cleanTags = dataFromRequest.Tags.Select(t => t.Trim()).ToList();
                    var existingDbTags = await tagRepo.GetAllQueryable()
                        .Where(t => cleanTags.Contains(t.Name))
                        .ToListAsync();

                    foreach (var tagName in cleanTags)
                    {
                        var tag = existingDbTags.FirstOrDefault(t => t.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase));
                        product.tags.Add(tag ?? new Tag { Name = tagName });
                    }
                }
            }

            // 4. انتظار كل العمليات الخارجية (Cloudinary والترجمة)
            if (tasks.Any())
            {
                await Task.WhenAll(tasks);
            }
        }

        private async Task UpdateDescriptionAsync(UpdateProductDto dto, bool isAr, Product p)
        {
            (string descAr, string descEn) = await TranslateDescription(dto, isAr);
            p.DescriptionAr = descAr.NormalizeArabicText();
            p.DescriptionEn = descEn;
        }

        private async Task UpdateImageAsync(UpdateProductDto dto, Product p)
        {
            if (!string.IsNullOrEmpty(p.ImageUrl))
            {
                string publicId = GetPublicIdFromUrl(p.ImageUrl);
                if (!string.IsNullOrEmpty(publicId))
                {
                     _cloudinary.DeleteAsync(publicId); // الـ await هنا ضروري جداً
                }
            }
            p.ImageUrl = await _cloudinary.UploadAsync(dto.ImageFile);
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
                CategoryName = category != null
                ? (isArabic ? category.NameAr : category.NameEn)
                : (isArabic ? "غير معروف" : "Unknown"),
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
        private static IEnumerable<ReturnProductDto> ReturnListDto(bool isArabic, IEnumerable<Product> products, Dictionary<int, ProductCategory> categoriesDict)
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
                CategoryName = categoriesDict.TryGetValue(p.CategoryId, out var category)
            ? (isArabic ? category.NameAr : category.NameEn) 
            : (isArabic ? "غير معروف" : "Unknown")
            }).ToList();
        }

    }
}
