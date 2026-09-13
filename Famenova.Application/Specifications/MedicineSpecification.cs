using famenova.Domain.Entities;
using Famenova.Shared.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Specifications
{
    public class MedicineSpecification : BaseSpecification<Medicine>
    {
        public MedicineSpecification(string? search, int? categoryId,bool? requiresPrescription, string? sort, int pageIndex, int pageSize)
        {
            Criteria = c => (string.IsNullOrEmpty(search) || c.Name.Contains(search)
            || (c.ActiveIngredient != null && c.ActiveIngredient.Contains(search)))
            && (!categoryId.HasValue || categoryId.Value == c.CategoryId)
            &&(!requiresPrescription.HasValue|| requiresPrescription.Value==c.RequiresPrescription);
            
            AddInclude(c => c.Category);

            if (String.IsNullOrWhiteSpace(sort))
            {
                AddOrderBy(c => c.Name);
            }
            else
            {
                switch (sort.ToLower())
                {
                    case "nameDesc":
                        AddOrderByDescending(c => c.Name);
                        break;
                    case "price":
                        AddOrderBy(c => c.Price);
                        break;

                    case "priceDesc":
                        AddOrderByDescending(c => c.Price);
                        break;
                    case "stockDesc":
                        AddOrderByDescending(c => c.Stock);
                        break;
                    default:
                        AddOrderBy(c => c.Name);
                        break;


                }

            }
            ApplyPaging(pageIndex, pageSize);

        }

        public MedicineSpecification(int id)
        {
            Criteria = c => c.Id == id;
           
            AddInclude(m => m.Category);
            
        }
    }
}
