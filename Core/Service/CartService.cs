using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.CartModule;
using DomainLayer.Models.Items;
using Microsoft.EntityFrameworkCore;
using ServiceAbstraction;
using Shared.BasketModule;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class CartService : ICartService
    {
        private readonly ICacheRepository _cacheRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public CartService(IUnitOfWork unitOfWork,ICacheRepository cacheRepository,IMapper mapper)
        {
            _cacheRepository = cacheRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<CartDto> AddItemToCartAsync(string cartId, int  itemId)
        {
            var product = await _unitOfWork.GetRepository<Product, int>()
                .GetAllQueryable() 
                .Include(p => p.Category) 
                .FirstOrDefaultAsync(p => p.Id == itemId);

            if (product == null) throw new ItemNotFound($"{itemId}");

            var cart = await GetOrCreateCartAsync(cartId);
            var item = new CartItem
            {
                Id = product.Id,
                ItemNameAr = product.NameAr,
                ItemNameEn = product.NameEn,
                Price = product.Price,
                Quantity = 1,
                PictureURL = product.ImageUrl,
                Category = product.Category.Name,
                CategoryId = product.CategoryId
            };

            ApplyItemToCart(cart, item);

            await _cacheRepository.AddOrUpdateAsync(cart.Id, cart);

            return _mapper.Map<CartDto>(cart);
              
           
        }

        public async Task<CartDto> UpdateQuantityAsync(string cartId, int productId, bool isIncrement)
        {
            var cart = await _cacheRepository.GetAsync<CustomerCart>(cartId);
            if (cart == null) throw new  CartNotFoundException(cartId);

            HandleQuantityUpdate(cart, productId, isIncrement);
            var item = cart.cartItems.FirstOrDefault(x => x.Id == productId);
            if (item != null)
            {
                var product = await _unitOfWork.GetRepository<Product, int>().GetByIdAsync(productId);
                if (product != null)
                {
                    item.ItemNameAr = product.NameAr;
                    item.ItemNameEn = product.NameEn;
                    item.Price = product.Price;
                }
            }
            await _cacheRepository.AddOrUpdateAsync(cartId, cart);
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> RemoveItemFromCartAsync(string cartId, int productId)
        {
            var cart = await _cacheRepository.GetAsync<CustomerCart>(cartId);

            if (cart != null)
            {
                var itemToRemove = cart.cartItems.FirstOrDefault(x => x.Id == productId);

                if (itemToRemove != null)
                {
                    cart.cartItems.Remove(itemToRemove);

                    await _cacheRepository.AddOrUpdateAsync(cartId, cart);
                }
            }

            return _mapper.Map<CartDto>(cart);
        }
        public async Task<CartDto> GetCartAsync(string id)
        {
            var basket = await _cacheRepository.GetAsync<CustomerCart>(id);
            if (basket == null)
            {
                return new CartDto { Id = id, cartItems = new List<CartItemDto>() };
            }
            return _mapper.Map<CartDto>(basket);
        }


        private async Task<CustomerCart> GetOrCreateCartAsync(string cartId)
        {
            var cart = await _cacheRepository.GetAsync<CustomerCart>(cartId);

            return cart ?? new CustomerCart { Id = cartId, cartItems = new List<CartItem>() };
        }

        private void ApplyItemToCart(CustomerCart cart, CartItem item)
        {
            var existingItem = cart.cartItems.FirstOrDefault(x => x.Id == item.Id);

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cart.cartItems.Add(item);
            }
        }

        private void HandleQuantityUpdate(CustomerCart cart, int productId, bool isIncrement)
        {
            var item = cart.cartItems.FirstOrDefault(x => x.Id == productId);

            if (item == null) return;

            if (isIncrement)
                item.Quantity++;
           
            else
            {
              
                item.Quantity--;

                if (item.Quantity <= 0)
                {
                    cart.cartItems.Remove(item);
                }
            }
        }

    }
}
