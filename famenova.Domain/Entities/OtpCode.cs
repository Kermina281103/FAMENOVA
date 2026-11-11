using famenova.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Entities
{

    public class OtpCode
    {
        public int Id { get; set; }
        public int UserId { get; set; }                  // الكود ده لمين
        public string CodeHash { get; set; } = default!; // الكود بعد ما اتشفّر
        public DateTime ExpiresAtUtc { get; set; }       // ينتهي إمتى
        public int FailedAttempts { get; set; }          // كام مرة اتكتب غلط
        public bool IsUsed { get; set; }                 // اتستخدم ولا لأ
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
