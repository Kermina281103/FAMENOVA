using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Entities
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TokenHash { get; set; } = default!;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime? RevokedAtUtc { get; set; }
        public string? ReplacedByTokenHash { get;private set; }
   
    private RefreshToken() { }

        public static RefreshToken Create(int userId, string tokenHash, DateTime expiresAtUtc) => new()
        {
            UserId = userId,
            TokenHash = tokenHash,
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = expiresAtUtc
        };

        public void Revoke(string? replaceByHash = null)
        {
            RevokedAtUtc = DateTime.UtcNow;
            ReplacedByTokenHash = replaceByHash;
        }

    }
}
