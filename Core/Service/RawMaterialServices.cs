using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.Categories;
using DomainLayer.Models.Identity;
using DomainLayer.Models.Items;
using DomainLayer.Models.RawMaterials;
using GTranslate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ServiceAbstraction;
using Shared.Extensions;
using Shared.IdentityModule;
using Shared.ProductModule;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class RawMaterialServices:IRawMaterialServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICloudinaryService _cloudinary;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITranslationService _translationService;
        public RawMaterialServices(ITranslationService translationService,IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager , ICloudinaryService cloudinary,IHttpContextAccessor httpContextAccessor) 
        { 
            _userManager = userManager;

            _unitOfWork = unitOfWork;
            _cloudinary = cloudinary;
            _httpContextAccessor = httpContextAccessor;
            _translationService = translationService;

        }

        public async Task<IEnumerable<ReturnProductDto>> GetAllMaterialsAsync()
        {
            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");


            var MaterialsRepo = _unitOfWork.GetRepository<RawMaterial, int>();
            var Materials = await MaterialsRepo.GetAllAsync();

            var categoryRepo = _unitOfWork.GetRepository<Raw_Category_Material, int>();
            var categories = await categoryRepo.GetAllAsync();

            var categoriesDict = categories.ToDictionary(c => c.Id, c => c.Name);

            return ReturnListDto(Materials, categoriesDict);
        }
        public async Task<ReturnProductDto> GetMaterialsByIdAsync(int id)
        {
            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");

            var MaterialsRepo = _unitOfWork.GetRepository<RawMaterial, int>();
            var Materials = await MaterialsRepo.GetByIdAsync(id);

            if (Materials == null)
                throw new ItemNotFound("this raw material not found");

            var categoryRepo = _unitOfWork.GetRepository<Raw_Category_Material, int>();
            var category = await categoryRepo.GetByIdAsync(Materials.CategoryId);
            var user =await _userManager.FindByIdAsync(Materials.supplierId);
            var userName = user!.FirstName + " " + user.SecondName;

            return ReturnDto(isArabic, Materials, category);

        }
        public async Task<ReturnProductDto> AddMaterialsAsync(CreateProductDto dto)
        {
            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");

            string? sellerId = AuthFun(isArabic);

            string imageUrl;
            if (dto.ImageFile != null)
            {

                imageUrl = await _cloudinary.UploadAsync(dto.ImageFile);
            }

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


            var material = new RawMaterial
            {
                NameEn = dto.NameEn,
                NameAr = dto.NameAr.NormalizeArabicText() ?? dto.NameAr,
                Price = dto.Price,
                Quantity = dto.Quantity ?? 0,
                DescriptionAr = DescAr,
                DescriptionEn = DescEn,
                CategoryId = dto.CategoryId,
                supplierId = sellerId,
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

                    if (tag != null) material.tags.Add(tag);
                    else
                    {
                        var newTag = new Tag { Name = cleanTagName };
                        material.tags.Add(newTag);
                    }
                }
            }

            await _unitOfWork.GetRepository<RawMaterial, int>().AddAsync(material);

            await _unitOfWork.SaveChanges();

            var categoryRepo = _unitOfWork.GetRepository<Raw_Category_Material, int>();
            var category = await categoryRepo.GetByIdAsync(dto.CategoryId);

            return ReturnDto(isArabic, material, category);
        }
        public async Task<ReturnProductDto> UpdateMaterialsAsync(int id, UpdateProductDto dataFromRequest)
        {
            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");

            var MaterialsRepo = _unitOfWork.GetRepository<RawMaterial, int>();
            var Materials = await MaterialsRepo.GetByIdAsync(id);

            if (Materials == null)
            {
                throw new ItemNotFound("this product not found");
            }
            ;

            Materials.NameEn = dataFromRequest.NameEn ?? Materials.NameEn ;
            Materials.NameAr = dataFromRequest.NameAr.NormalizeArabicText() ?? Materials.NameAr ;
            Materials.Price = dataFromRequest.Price ?? Materials.Price ;

            if (dataFromRequest.Price.HasValue && dataFromRequest.Price > 0)
                Materials.Price = dataFromRequest.Price.Value;


            if (dataFromRequest.CategoryId.HasValue && dataFromRequest.CategoryId > 0)
                Materials.CategoryId = dataFromRequest.CategoryId.Value;
            if (!string.IsNullOrWhiteSpace(dataFromRequest.Description))
            {
                (string descAr, string descEn) = await TranslateDescription(dataFromRequest, isArabic);
                Materials.DescriptionAr = descAr;
                Materials.DescriptionEn = descEn;
            }


            if (dataFromRequest.ImageFile != null)
            {

                if (!string.IsNullOrEmpty(Materials.ImageUrl))
                {
                    string publicId = GetPublicIdFromUrl(Materials.ImageUrl);
                    if (!string.IsNullOrEmpty(publicId))
                    {
                        _cloudinary.DeleteAsync(publicId);
                    }
                }

                Materials.ImageUrl = await _cloudinary.UploadAsync(dataFromRequest.ImageFile);
            }

            MaterialsRepo.Update(Materials);
            await _unitOfWork.SaveChanges();

            var categoryRepo = _unitOfWork.GetRepository<Raw_Category_Material, int>();
            var category = await categoryRepo.GetByIdAsync(Materials.CategoryId);

            return ReturnDto(isArabic,Materials,category);
        }
        public async Task<bool> DeleteMaterialsAsync(int id)
        {
            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");
            AuthFun(isArabic);
            var repo = _unitOfWork.GetRepository<RawMaterial, int>();
            var material = await repo.GetByIdAsync(id);
            if (material == null)
            {
                throw new ItemNotFound("this item is already not found");
            }


            if (!string.IsNullOrEmpty(material.ImageUrl))
            {
                string publicId = GetPublicIdFromUrl(material.ImageUrl);
                if (!string.IsNullOrEmpty(publicId))
                {
                    _cloudinary.DeleteAsync(publicId);
                }
            }

            repo.Remove(material);
            await _unitOfWork.SaveChanges();
            return true;
        }
        public async Task<IEnumerable<ReturnProductDto>> GetAllMaterialsOfSpecifiUserAsync(string id)
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
                if (role != RoleType.Supplier.ToString())
                {
                    throw new InvalidOperationException("role is not valid to do this operation");
                }
            }
            var repo = _unitOfWork.GetRepository<RawMaterial, int>();
            var query = await repo.GetAllAsync();
            var Materials = query.Where(p => p.supplierId == id);

            var categoryRepo = _unitOfWork.GetRepository<Raw_Category_Material, int>();
            var categories = await categoryRepo.GetAllAsync();

            var categoriesDict = categories.ToDictionary(c => c.Id, c => c.Name);

            return ReturnListDto(Materials, categoriesDict);


        }
        public async Task<int> GetMaterialsCountByUserIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new UserNotFoundException("this user is not found");
            }

            // 2. الحصول على الـ Repository وعمل Count للمنتجات الخاصة بهذا المستخدم
            var repo = _unitOfWork.GetRepository<RawMaterial, int>();
            var query = await repo.GetAllAsync();

            return query.Count(p => p.supplierId == userId);
        }
        public async Task<IEnumerable<ReturnProductsOfCategory>> GetAllMaterialsOfSpecificCategory(int id)
        {
            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");

            var repo = _unitOfWork.GetRepository<RawMaterial, int>();
            var query = await repo.GetAllAsync();
            return query
                    .Where(p => p.CategoryId == id)
                    .Select(p => new ReturnProductsOfCategory
                    {
                        Id = p.Id,
                        Image = p.ImageUrl ?? "",
                        Name = isArabic ? p.NameAr : p.NameEn,
                        Description = isArabic! ? p.DescriptionAr! : p.DescriptionEn!,
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
                DescEn = await _translationService.TranslateAsync(dataFromRequest.Description!, "en");
            }
            else
            {
                DescEn = dataFromRequest?.Description!;
                DescAr = await _translationService .TranslateAsync(dataFromRequest?.Description!, "ar");
            }

            return (DescAr!, DescEn);
        }
        private static ReturnProductDto ReturnDto(bool isArabic, RawMaterial material, Raw_Category_Material category)
        {
            return new ReturnProductDto
            {

                Id = material.Id,
                Name = isArabic ? material.NameAr : material.NameEn,
                Price = material.Price,
                Quantity = material.Quantity ?? 0,
                Description = isArabic ? material.DescriptionAr : material.DescriptionEn,
                ImageUrl = material.ImageUrl,
                CategoryId = material.CategoryId,
                SellerId = material.supplierId,
                CategoryName = category != null ? category.Name : "",
                SellerName = material.supplier?.FirstName + " " + material.supplier?.SecondName,
            };
        }
        private static IEnumerable<ReturnProductDto> ReturnListDto(IEnumerable<RawMaterial> Materials, Dictionary<int, string> categoriesDict)
        {
            return Materials.Select(p => new ReturnProductDto
            {
                Id = p.Id,
                Name = p.NameEn,
                Price = p.Price,
                Quantity = p.Quantity ?? 0,
                Description = p.DescriptionEn,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                SellerId = p.supplierId,
                CategoryName = categoriesDict.ContainsKey(p.CategoryId)
                                           ? categoriesDict[p.CategoryId]
                                           : "Unknown"
            }).ToList();
        }
        private string AuthFun(bool isArabic)
        {
            var sellerId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(sellerId))
                throw new UnauthorizedAccessException(isArabic ? "يجب تسجيل الدخول أولاً" : "Unauthorized: Please login");

            var user = _httpContextAccessor?.HttpContext?.User;

            bool isAuthorizedRole = user!.IsInRole(RoleType.Supplier.ToString());


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

    }
}
