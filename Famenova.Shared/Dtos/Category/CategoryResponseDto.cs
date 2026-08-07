using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Dtos.Category
{
    public class CategoryResponseDto
    {
        
            public int Id { get; set; }

            public string Name { get; set; } = string.Empty;

            public string? ImageUrl { get; set; }

            public string? Description { get; set; }

            public bool IsActive { get; set; }

            public int DisplayOrder { get; set; }
        
    }
}
