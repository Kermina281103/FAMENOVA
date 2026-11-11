using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Jwt
{
    public class JwtOptions
    {
        public const string SectionName = "Jwt";
        public string Key { get; set; } = default!;
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public int AccessTokenMinutes { get; set; } = 30;
        public int RefreshTokenDays { get; set; } = 7;
    }
    public record AccessToken(string value , DateTime ExpiresAtUtc );

}
