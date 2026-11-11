using famenova.Domain.Entities;
using Famenova.Application.Interfaces;
using Famenova.Application.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Infrastructure.Jwt
{
    public class JwtTokenGenerator:IJwtTokenGenerator
    {
        private readonly JwtOptions _options;

        public JwtTokenGenerator(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }

        public AccessToken Create(ApplicationUser user, IEnumerable<string> roles)
        {
            var claims = new List<Claim>
          {
              new(ClaimTypes.NameIdentifier,user.Id.ToString()),
              new(ClaimTypes.Email,user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
          };

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials);

                return new AccessToken(new JwtSecurityTokenHandler().WriteToken(token), expires);

        }
    }
}
