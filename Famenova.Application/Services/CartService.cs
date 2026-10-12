using AutoMapper;
using famenova.Domain.Entities;
using famenova.Domain.Interfaces;
using Famenova.Application.Exceptions;
using Famenova.Application.Interfaces;
using Famenova.Shared.Dtos.Cart;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Services
{
    class CartService : ICartService
    {
        private readonly ICartRepository _carRepo;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public CartService(ICartRepository cartRepo, IMapper mapper, IConfiguration config)
        {
            _carRepo = cartRepo;
            _mapper = mapper;
            _config = config;
        
        }
        public async Task<CartDto> GetCartAsync(string cartId)
        {
            var cart = await _carRepo.GetByIdAsync(cartId);
            if (cart is null)
                throw new NotFoundException($"Not Found Cart With Id {cartId} ");
            return _mapper.Map<CartDto>(cart);
        }

        public async  Task<CartDto> UpdateCartAsync(CartDto dto)
        {
            if (string.IsNullOrEmpty(dto.Id))
                dto.Id = Guid.NewGuid().ToString();


            var cart = _mapper.Map<Cart>(dto);
            var timeToLive = TimeSpan.FromDays(double.Parse
                (_config["RedisSettings:TimeToLiveDays"]!));

            var savedCart = await _carRepo.UpdateAsync(cart, timeToLive);
            if (savedCart is null)
                throw new BadRequestException("there is a problem in saving cart");

            return _mapper.Map<CartDto>(savedCart);

        }
         public async Task DeleteCartAsync(string cartId)
        {
            var cart = await _carRepo.GetByIdAsync(cartId);
            if (cart is null)
                throw new NotFoundException($"Not Found Cart With Id {cartId} ");
           var deleted =await  _carRepo.DeleteAsync(cartId);
            if (!deleted)
                throw new BadRequestException("unable to delete this cart ");

        }



    }
}
