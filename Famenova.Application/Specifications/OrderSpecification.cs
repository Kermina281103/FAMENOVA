using famenova.Domain.Entities;
using famenova.Domain.Enums;
using Famenova.Application.Specifications.param;
using Famenova.Shared.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Specifications
{
    internal static class OrderCriteria
    {
        public static Expression<Func<Order,bool>> ForAdmin(OrderSpecParams spec)
        {
            var trim = spec.search?.Trim();

            return c =>
        (!spec.customerId.HasValue || c.UserId == spec.customerId.Value) &&
        (!spec.paymentStatus.HasValue || c.PaymentStatus == spec.paymentStatus.Value) &&
        (!spec.paymentMethod.HasValue || c.PaymentMethod == spec.paymentMethod.Value) &&
        (!spec.orderStatus.HasValue || c.OrderStatus == spec.orderStatus.Value) &&
        (!spec.source.HasValue || c.Source == spec.source.Value) &&
        (string.IsNullOrEmpty(trim)
            || c.PhoneNumber.Contains(trim)
            || c.User.UserName!.Contains(trim));


        }
        public static Expression<Func<Order,bool>> ForCustomer(int UserId,OrderSpecParams spec)
        {
            return x => (x.UserId == UserId) &&
            (!spec.paymentStatus.HasValue || spec.paymentStatus == x.PaymentStatus) &&
            (!spec.orderStatus.HasValue || spec.orderStatus == x.OrderStatus);
        }

       
    }
    public class OrderSpecification:BaseSpecification<Order>
    {
        public OrderSpecification(OrderSpecParams spec)
        {
            Criteria = OrderCriteria.ForAdmin(spec);
            AddInclude(query => query.Include(x => x.ShippingAddress));
            AddInclude(query => query.Include(x => x.OrderItems).ThenInclude(o => o.Product));

            ApplySort(spec);
            ApplyPaging(spec.pageIndex, spec.pageSize);
        }
        public OrderSpecification(int id)
        {
            Criteria = o => o.Id == id;
            AddInclude(q => q.Include(o => o.User));
            AddInclude(q => q.Include(o => o.OrderItems).ThenInclude(i => i.Product));
        }
      

        private void ApplySort(OrderSpecParams spec)
        {
            switch (spec.sort?.ToLower())
            {
                case "orderdate": AddOrderBy(x => x.OrderDate); break;
                case "total": AddOrderBy(x => x.Total!); break;
                case "totaldesc": AddOrderByDescending(x => x.Total); break;
                case "paymentstatus": AddOrderBy(x => x.PaymentStatus); break;
                case "paymentstatusdesc": AddOrderByDescending(x => x.PaymentStatus); break;
                case "paymentethod": AddOrderBy(x => x.PaymentMethod!); break;
                case "paymentethoddesc": AddOrderByDescending(x => x.PaymentMethod!); break;
                case "orderstatus": AddOrderBy(x => x.OrderStatus); break;
                case "orderstatusdesc": AddOrderByDescending(x => x.OrderStatus); break;
                default:AddOrderBy(x => x.OrderDate);break;
            }
        }
    }

    public class MyOrdersSpecification : BaseSpecification<Order>
    {
        public MyOrdersSpecification(OrderSpecParams spec, int userId)
        {
            Criteria = OrderCriteria.ForCustomer(userId, spec);
            AddInclude(query => query.Include(x => x.ShippingAddress));
            AddInclude(query => query.Include(x => x.OrderItems).ThenInclude(o => o.Product));

            ApplySort(spec);
            ApplyPaging(spec.pageIndex, spec.pageSize);

        }
        private void ApplySort(OrderSpecParams spec)
        {
            switch (spec.sort?.ToLower())
            {
                case "orderdate": AddOrderBy(x => x.OrderDate); break;
                case "total": AddOrderBy(x => x.Total!); break;
                case "totaldesc": AddOrderByDescending(x => x.Total); break;
                default:AddOrderBy(x => x.OrderDate);break;
            }
        }
    }

    public class MyOrderDetailsSpecification : BaseSpecification<Order>
    {
        public MyOrderDetailsSpecification(int id, int userId)
        {
            Criteria = c => c.Id == id && c.UserId == userId;

            AddInclude(query => query.Include(x => x.User));

            AddInclude(query => query.Include(x => x.ShippingAddress));

            AddInclude(query =>
                query.Include(x => x.OrderItems)
                     .ThenInclude(o => o.Product));
        }
    }
}
