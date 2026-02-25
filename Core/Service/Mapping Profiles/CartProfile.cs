using AutoMapper;
using DomainLayer.Models.CartModule;
using Shared.BasketModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Mapping_Profiles
{
    public class CartProfile: Profile
    {
        public CartProfile() {
            CreateMap<CustomerCart, CartDto>().ReverseMap();
            CreateMap<CartItem,CartItemDto>().ReverseMap();
        }
    }
}
