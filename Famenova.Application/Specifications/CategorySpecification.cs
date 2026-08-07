using famenova.Domain.Entities;
using Famenova.Shared.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Specifications
{
    public class CategorySpecification:BaseSpecification<Category>
    {
        public CategorySpecification(string? search, string? sort ,int pageIndex, int pageSize)
        {
            if (!string.IsNullOrWhiteSpace(search))
            {
                Criteria = c => c.Name.Contains(search);
            }
            if (string.IsNullOrWhiteSpace(sort))
            {
                AddOrderBy(c => c.Name);
            }
            else { 
                switch (sort?.ToLower())
                { 
                    case "nameDesc":
                        AddOrderByDescending(c => c.Name);
                        break;
                    case "displayOrder":
                        AddOrderBy(c => c.DisplayOrder);
                        break;
                    default:
                        AddOrderBy(c => c.Name);
                        break;
                }


            }
            ApplyPaging(pageIndex,pageSize);
        }
    }
}
