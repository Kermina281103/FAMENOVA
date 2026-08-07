using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Common.Models
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; }
        public int PageNumber { get;  }
        public int PageSize { get;  }
        public int TotalCount { get;  }
        public int TotalPage { get; }

        public PagedResult(IEnumerable<T> items,int pageNumber,int pageSize,int totalCount)
        {
            Items = items;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPage = (int)Math.Ceiling((double)totalCount / pageSize);
          
            
        }

    }
}
