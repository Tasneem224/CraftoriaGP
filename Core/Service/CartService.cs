using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Models.CartModule;
using ServiceAbstraction;
using Shared.BasketModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class CartService : ICartService
    {
        private readonly ICacheRepository _cacheRepository;
        private readonly IMapper _mapper;
        public CartService(ICacheRepository cacheRepository,IMapper mapper)
        {
            _cacheRepository = cacheRepository;
            _mapper = mapper;
        }

        public async Task<CartDto> CreateOrUpdateCartAsync(CartDto cartDto)
        {
            var basket = _mapper.Map<CustomerCart>(cartDto);
             var createdOrUpdatedBasket= await _cacheRepository.AddOrUpdateAsync(basket.Id, basket);
            return createdOrUpdatedBasket is null ? throw new Exception("Can't create or update cart")
                :_mapper.Map<CartDto>(createdOrUpdatedBasket);
        }

        public async Task<bool> DeleteCartAsync(string id)
        {
           return await _cacheRepository.DeleteAsync(id);
        }

        public async Task<CartDto> GetCartAsync(string id)
        {
            var basket = await _cacheRepository.GetAsync<CustomerCart>(id);
            return basket is null ? throw new CartNotFoundException("Can't get Cart")  :_mapper.Map<CartDto>(basket);
        }
    }
}
