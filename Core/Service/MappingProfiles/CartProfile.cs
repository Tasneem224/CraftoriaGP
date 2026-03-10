using AutoMapper;
using DomainLayer.Models.CartModule;
using Shared.BasketModule;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Mapping_Profiles
{
    public class CartProfile: Profile
    {
        public CartProfile() {
            CreateMap<CustomerCart, CartDto>().ReverseMap();


            CreateMap<CartItem, CartItemDto>()
               .ForMember(d => d.ItemName, o => o.MapFrom(s =>
                   CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ar"
                   ? s.ItemNameAr
                   : s.ItemNameEn))
               .ForMember(d => d.Category, o => o.MapFrom(s =>
                CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ar"
                ?s.CategoryNameAr
                :s.CategoryNameEn));


            CreateMap<CartItemDto, CartItem>();
             
        }
    }
}
