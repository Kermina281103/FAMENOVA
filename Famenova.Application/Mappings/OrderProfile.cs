using AutoMapper;
using famenova.Domain.Entities;
using Famenova.Shared.Dtos.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Mappings
{
    public class OrderProfile:Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderResponseDto>()
                .ForMember(d => d.Items, sr => sr.MapFrom(x => x.OrderItems))
                .ForMember(d => d.OrderStatus, sr => sr.MapFrom(x => x.OrderStatus.ToString()))
                .ForMember(d => d.PaymentStatus, sr => sr.MapFrom(x => x.PaymentStatus))
                .ForMember(d => d.Total, sr => sr.MapFrom(x => x.OrderItems.Sum(i => i.Quantity * i.UnitPrice)));

            CreateMap<OrderItem, OrderItemResponseDto>()
                .ForMember(de => de.ProductName, sr => sr.MapFrom(x => x.Product!.Name))
                .ForMember(de => de.SubTotal, sr => sr.MapFrom(x => (x.UnitPrice * x.Quantity)));
        }
    }
}