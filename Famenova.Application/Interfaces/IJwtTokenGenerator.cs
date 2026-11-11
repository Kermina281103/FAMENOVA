using famenova.Domain.Entities;
using Famenova.Application.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        AccessToken Create(ApplicationUser user, IEnumerable<string> roles);
    }
}
