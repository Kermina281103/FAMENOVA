using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Infrastructure.Payments
{
    public static class PaymentSignature
    {
        public static string FormatAmount(decimal amount)
            => amount.ToString("F2", CultureInfo.InvariantCulture);

        public static string Compute(params string[] parts)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(string.Concat(parts)));
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        public static bool AreEqual(string a, string b)
            => CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(a.ToLowerInvariant()),
                Encoding.UTF8.GetBytes(b.ToLowerInvariant()));
        
    }
}
