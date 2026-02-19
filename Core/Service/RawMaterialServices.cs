using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.Categories;
using DomainLayer.Models.Identity;
using DomainLayer.Models.Items;
using DomainLayer.Models.RawMaterials;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ServiceAbstraction;
using Shared.IdentityModule;
using Shared.ProductModule;
using System;
using System.Collections.Generic;
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
        public RawMaterialServices(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager , ICloudinaryService cloudinary,IHttpContextAccessor httpContextAccessor) 
        { 
            _userManager = userManager;

            _unitOfWork = unitOfWork;
            _cloudinary = cloudinary;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ReturnProductDto> AddMaterialsAsync(CreateProductDto dto)
        {
            var isArabic = Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar");

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

            string imageUrl = null;
            if (dto.ImageFile != null)
            {

                imageUrl = await _cloudinary.UploadAsync(dto.ImageFile);
            }

            else
            {
                throw new Exception("you should upload image");
            }


            var product = new RawMaterial
            {
                NameEn = dto.NameEn,
                Price = dto.Price,
                Quantity = dto.Quantity ?? 0,
                DescriptionEn = dto.Description,
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

                    if (tag != null) product.tags.Add(tag);
                    else
                    {
                        var newTag = new Tag { Name = cleanTagName };
                        product.tags.Add(newTag);
                    }
                }
            }


            await _unitOfWork.GetRepository<RawMaterial, int>().AddAsync(product);
          
                await _unitOfWork.SaveChanges();
           
            // 4. Get Category Name for response
            var categoryRepo = _unitOfWork.GetRepository<Raw_Category_Material, int>();
            var category = await categoryRepo.GetByIdAsync(dto.CategoryId);

            return new ReturnProductDto
            {
                Id = product.Id,
                Name = product.NameEn,
                Price = product.Price,
                Quantity = product.Quantity ?? 0,
                Description = product.DescriptionEn,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                SellerId = product.supplierId,
                CategoryName = category != null ? category.Name : ""
            };
        }

        public async Task<bool> DeleteMaterialsAsync(int id)
        {
            var repo = _unitOfWork.GetRepository<RawMaterial, int>();
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

        public async Task<IEnumerable<ReturnProductDto>> GetAllMaterialsAsync()
        {
            var MaterialsRepo = _unitOfWork.GetRepository<RawMaterial, int>();
            var Materials = await MaterialsRepo.GetAllAsync();

            var categoryRepo = _unitOfWork.GetRepository<Raw_Category_Material, int>();
            var categories = await categoryRepo.GetAllAsync();

            var categoriesDict = categories.ToDictionary(c => c.Id, c => c.Name);

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

        public async Task<IEnumerable<ReturnProductDto>> GetAllMaterialsOfSpecifiUserAsync(string id)
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

        public async Task<ReturnProductDto> GetMaterialsByIdAsync(int id)
        {
            var MaterialsRepo = _unitOfWork.GetRepository<RawMaterial, int>();
            var Materials = await MaterialsRepo.GetByIdAsync(id);

            if (Materials == null) return null;

            var categoryRepo = _unitOfWork.GetRepository<Raw_Category_Material, int>();
            var category = await categoryRepo.GetByIdAsync(Materials.CategoryId);

            return new ReturnProductDto
            {
                Id = Materials.Id,
                Name = Materials.NameEn,
                Price = Materials.Price,
                Quantity = Materials.Quantity ?? 0,
                Description = Materials.DescriptionEn,
                ImageUrl = Materials.ImageUrl,
                CategoryId = Materials.CategoryId,
                SellerId = Materials.supplierId,
                CategoryName = category != null ? category.Name : "Unknown"
            };
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
        public async Task<ReturnProductDto> UpdateMaterialsAsync(int id, UpdateProductDto dataFromRequest)
        {
            var MaterialsRepo = _unitOfWork.GetRepository<RawMaterial, int>();
            var Materials = await MaterialsRepo.GetByIdAsync(id);

            if (Materials == null)
            {
                throw new ItemNotFound("this product not found");
            }
            ;

            Materials.NameEn = dataFromRequest.NameEn == null ? Materials.NameEn : dataFromRequest.NameEn;
            Materials.Price = dataFromRequest.Price == 0.0m ? Materials.Price : dataFromRequest.Price;
            Materials.DescriptionEn = dataFromRequest.Description == null ? Materials.DescriptionEn : dataFromRequest.Description;
            Materials.CategoryId = dataFromRequest.CategoryId == 0 ? Materials.CategoryId : dataFromRequest.CategoryId;
            Materials.Quantity = dataFromRequest.Quantity == null ? Materials.Quantity : dataFromRequest.Quantity;
            //if (dataFromRequest.Quantity.HasValue) product.Quantity = dataFromRequest.Quantity.Value;


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

            var categoryRepo = _unitOfWork.GetRepository<RawMaterial, int>();
            var category = await categoryRepo.GetByIdAsync(Materials.CategoryId);

            return new ReturnProductDto
            {
                Id = Materials.Id,
                Name = Materials.NameEn,
                Price = Materials.Price,
                Quantity = Materials.Quantity ?? 0,
                Description = Materials.DescriptionEn,
                ImageUrl = Materials.ImageUrl,
                CategoryId = Materials.CategoryId,
                SellerId = Materials.supplierId,
                CategoryName = category != null ? category.NameEn : "Unknown"
            };
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

    }
}
