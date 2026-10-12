using AutoMapper;
using famenova.Domain.Entities;
using Famenova.Shared.Dtos.Cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Mappings
{
    public class CartProfile:Profile
    {
        public CartProfile()
        {
            CreateMap<CartItem, CartItemDto>().ReverseMap();
            CreateMap<Cart, CartDto>().ReverseMap();
           
        }
    }
}
