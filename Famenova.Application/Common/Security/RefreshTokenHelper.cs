using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Common.Security
{
    public static class RefreshTokenHelper
    {
        public static string Generate()
          => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
              .Replace('+', '-').Replace('/', '_').TrimEnd('=');

        public static string Hash(string token)
            => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
}
