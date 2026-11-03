using famenova.Domain.Entities;
using Famenova.Shared.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Famenova.Application.Specifications
{
    public class MedicineSpecification : BaseSpecification<Medicine>
    {
        // ===== قايمة الأدوية =====
        public MedicineSpecification(string? search, int? categoryId, bool? requiresPrescription, string? sort, int pageIndex, int pageSize)
        {
            var trim = search?.Trim();   // 🆕 زي المنتجات

            Criteria = c => (string.IsNullOrEmpty(trim) || c.Name.Contains(trim)
                || (c.ActiveIngredient != null && c.ActiveIngredient.Contains(trim)))
                && (!categoryId.HasValue || categoryId.Value == c.CategoryId)
                && (!requiresPrescription.HasValue || requiresPrescription.Value == c.RequiresPrescription);

            AddInclude(query => query.Include(x => x.Category));

            switch (sort?.ToLower())
            {
                case "namedesc":                                   // 🐛 كانت nameDesc
                    AddOrderByDescending(c => c.Name);
                    break;
                case "price":
                    AddOrderBy(c => c.Price);
                    break;
                case "pricedesc":                                  // 🐛 كانت priceDesc
                    AddOrderByDescending(c => c.Price);
                    break;
                case "stockdesc":                                  // 🐛 كانت stockDesc
                    AddOrderByDescending(c => c.Stock);
                    break;
                default:
                    AddOrderBy(c => c.Name);
                    break;
            }

            ApplyPaging(pageIndex, pageSize);
        }

        // ===== دوا واحد بالـ Id =====
        public MedicineSpecification(int id)
        {
            Criteria = c => c.Id == id;
            AddInclude(query => query.Include(x => x.Category));
        }

        // 🆕 ===== كل التركيزات/الأحجام في نفس المجموعة =====
        private MedicineSpecification() { }

        public static MedicineSpecification Variants(int rootId)
        {
            var spec = new MedicineSpecification();
            spec.Criteria = m => !m.IsDeleted && (m.Id == rootId || m.ProductGroupId == rootId);
            spec.AddOrderBy(m => m.Price);
            return spec;
        }
    }
}