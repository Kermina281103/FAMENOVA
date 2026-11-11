using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared.Dtos.Auth
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = default!;
        public DateTime ExpiresAtUtc { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public IList<string> Roles { get; set; } = new List<string>();
        public string RefreshToken { get; set; } = default!;
        public DateTime RefreshTokenExpiresAtUtc { get; set; }
    }
}
