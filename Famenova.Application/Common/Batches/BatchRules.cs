using Famenova.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Common.Batches
{
    internal static class BatchRules
    {
        public const int MaxNumberLength = 50;

        public static string ValidateNumber(string? batchNumber)
        {
            var n = batchNumber?.Trim();

            if (string.IsNullOrEmpty(n))
                throw new BadRequestException("Batch number is required.");

            if (n.Length > MaxNumberLength)
                throw new BadRequestException(
                    "Batch number must be 50 characters or fewer.");

            return n;
        }

        public static DateTime ValidateNewExpiry(DateTime expiry)
        {
            if (expiry.Date <= DateTime.UtcNow.Date)
                throw new BadRequestException(
                    "Expiry date must be after today.");

            return expiry.Date;
        }

        public static void ValidateQuantity(int quantity, bool isNew)
        {
            if (isNew ? quantity < 1 : quantity < 0)
                throw new BadRequestException(
                    isNew ? "Quantity must be at least 1." :
                            "Quantity can't be negative.");
        }
    }
}
