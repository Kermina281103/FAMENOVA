using famenova.Domain.Entities;
using famenova.Domain.Enums;
using Famenova.Shared.Specifications;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Famenova.Application.Specifications
{
    internal static class PrescriptionOrderCriteria
    {
        public static Expression<Func<PrescriptionOrder, bool>> ForAdmin(
            string? search, int? customerId, PrescriptionStatus? status,
            bool? hasClarificationResponse)
        {
            var term = search?.Trim();

            return x =>
                (!customerId.HasValue || x.CustomerId == customerId.Value) &&
                (!status.HasValue || x.Status == status.Value) &&
                ((string.IsNullOrEmpty(term) ||
              x.PhoneNumber.Contains(term) || (x.Customer.UserName != null && x.Customer.UserName.Contains(term)) 
            ))
                && (!hasClarificationResponse.HasValue || (hasClarificationResponse.Value? x.ClarificationRespondedAt != null
                                                         : x.ClarificationRespondedAt == null) ) ;
        }

        public static Expression<Func<PrescriptionOrder, bool>> ForCustomer(
            int customerId, PrescriptionStatus? status)
        {
            return x =>
                x.CustomerId == customerId &&
                (!status.HasValue || x.Status == status.Value);
        }
    }

    // ================= Admin: List =================
    public class PrescriptionOrderSpecification : BaseSpecification<PrescriptionOrder>
    {
        public PrescriptionOrderSpecification(PrescriptionOrderSpecParams specParams)
        {
            Criteria = PrescriptionOrderCriteria.ForAdmin(specParams.search, specParams.customerId, specParams.status,specParams.HasClarificationResponse);

            AddInclude(q => q.Include(x => x.Customer)); 

            switch (specParams.sort?.ToLower())
            {
                case "createdondesc": AddOrderByDescending(x => x.CreatedOn); break;
                case "status": AddOrderBy(x => x.Status); break;
                case "statusdesc": AddOrderByDescending(x => x.Status); break;
                case "totalprice": AddOrderBy(x => x.TotalPrice); break;
                case "totalpricedesc": AddOrderByDescending(x => x.TotalPrice); break;
                default: AddOrderBy(x => x.CreatedOn); break; 
            }

            ApplyPaging(specParams.pageIndex,specParams.pageSize);
        }

        // ================= Admin: Details / Commands =================
        public PrescriptionOrderSpecification(int id)
        {
            Criteria = x => x.Id == id;

            AddInclude(q => q.Include(x => x.Customer));
            AddInclude(q => q.Include(x => x.Items).ThenInclude(i => i.Product));
        }
    }

    // ================= Admin: Count =================
    public class PrescriptionOrderCountSpecification : BaseSpecification<PrescriptionOrder>
    {
        public PrescriptionOrderCountSpecification(
            string? search, int? customerId, PrescriptionStatus? status,bool? hasClarificationResponse)
        {
            Criteria = PrescriptionOrderCriteria.ForAdmin(search, customerId, status, hasClarificationResponse);
        }
    }

    // ================= Customer: List =================
    public class MyPrescriptionsSpecification : BaseSpecification<PrescriptionOrder>
    {
        public MyPrescriptionsSpecification(
           PrescriptionOrderSpecParams specParams, int customerId)
        {
            Criteria = PrescriptionOrderCriteria.ForCustomer(customerId, specParams.status);
            AddInclude(query => query.Include(x => x.Customer));
            switch (specParams.sort?.ToLower())
            {
                case "createdon": AddOrderBy(x => x.CreatedOn); break;
                case "status": AddOrderBy(x => x.Status); break;
                case "statusdesc": AddOrderByDescending(x => x.Status); break;
                default: AddOrderByDescending(x => x.CreatedOn); break; // الأحدث الأول
            }

            ApplyPaging(specParams.pageIndex,specParams.pageSize);
        }
    }

    // ================= Customer: Count =================
    public class MyPrescriptionsCountSpecification : BaseSpecification<PrescriptionOrder>
    {
        public MyPrescriptionsCountSpecification(int customerId, PrescriptionStatus? status)
        {
            Criteria = PrescriptionOrderCriteria.ForCustomer(customerId, status);
        }
    }

    // ================= Customer: Details / Commands =================
    public class MyPrescriptionDetailsSpecification : BaseSpecification<PrescriptionOrder>
    {
        public MyPrescriptionDetailsSpecification(int id, int customerId)
        {
            Criteria = x => x.Id == id && x.CustomerId == customerId;

            AddInclude(q => q.Include(x => x.Items).ThenInclude(i => i.Product));
        }
    }
}