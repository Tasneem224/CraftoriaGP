using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.Categories;
using DomainLayer.Models.Identity;
using DomainLayer.Models.Items;
using Microsoft.AspNetCore.Identity;
using ServiceAbstraction;
using Shared.IdentityModule;
using Shared.ProductModule;

namespace Service
{
    public class ProductService(UserManager<ApplicationUser> _userManager, ICloudinaryService _cloudinary, IUnitOfWork _unitOfWork) : IProductService
    {
        public async Task<IEnumerable<ReturnProductDto>> GetAllProductsAsync()
        {
            var productRepo = _unitOfWork.GetRepository<Product, int>();
            var products = await productRepo.GetAllAsync();

            var categoryRepo = _unitOfWork.GetRepository<Category, int>();
            var categories = await categoryRepo.GetAllAsync();

            var categoriesDict = categories.ToDictionary(c => c.Id, c => c.Name);

            return products.Select(p => new ReturnProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Quantity = p.Quantity ?? 0,
                Description = p.Description,
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
            var productRepo = _unitOfWork.GetRepository<Product, int>();
            var product = await productRepo.GetByIdAsync(id);

            if (product == null) return null;

            var categoryRepo = _unitOfWork.GetRepository<Category, int>();
            var category = await categoryRepo.GetByIdAsync(product.CategoryId);

            return new ReturnProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Quantity = product.Quantity ?? 0,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                SellerId = product.SellerId,
                CategoryName = category != null ? category.Name : "Unknown"
            };
        }

        public async Task<ReturnProductDto> AddProductAsync(CreateProductDto dto, string sellerId)
        {
            var checkUser = await _userManager.FindByIdAsync(sellerId);
            if (checkUser is null)
            {
                throw new UserNotFoundException("this user is not found");

            }
            var user =await  _userManager.FindByIdAsync(sellerId);
            var roles =await  _userManager.GetRolesAsync(user);
            foreach(var  role in roles)
            {
                if (role != RoleType.Beginner.ToString() && role != RoleType.Expert.ToString())
                {
                    throw new InvalidOperationException("role is not valid to do this operation");
                }
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


                var product = new Product
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    Quantity = dto.Quantity ?? 0,
                    Description = dto.Description,
                    CategoryId = dto.CategoryId,
                    SellerId = sellerId,
                    ImageUrl = imageUrl
                };


            await _unitOfWork.GetRepository<Product, int>().AddAsync(product);
            await _unitOfWork.SaveChanges();

            // 4. Get Category Name for response
            var categoryRepo = _unitOfWork.GetRepository<Category, int>();
            var category = await categoryRepo.GetByIdAsync(dto.CategoryId);

            return new ReturnProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Quantity = product.Quantity ?? 0,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                SellerId = product.SellerId,
                CategoryName = category != null ? category.Name : ""
            };
        }

        public async Task<ReturnProductDto> UpdateProductAsync(int id, UpdateProductDto dataFromRequest, string sellerId)
        {
            var repo = _unitOfWork.GetRepository<Product, int>();
            var product = await repo.GetByIdAsync(id);

            if (product == null)
            {
                throw new ItemNotFound("this product not found");
            }
            ;

            //if (product.SellerId != sellerId)
            //    throw new Exception("Unauthorized: You can only update your own products.");
            

            
            product.Name = dataFromRequest.Name==null?product.Name: dataFromRequest.Name;
            product.Price = dataFromRequest.Price==0.0m?product.Price: dataFromRequest.Price;
            product.Description = dataFromRequest.Description==null?product.Description: dataFromRequest.Description;
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

            var categoryRepo = _unitOfWork.GetRepository<Category, int>();
            var category = await categoryRepo.GetByIdAsync(product.CategoryId);

            return new ReturnProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Quantity = product.Quantity ?? 0,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                SellerId = product.SellerId,
                CategoryName = category != null ? category.Name : "Unknown"
            };
        }

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

    }
}
