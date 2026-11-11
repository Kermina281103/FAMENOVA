using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Otp
{
    public class OtpOptions
    {
        public const string SectionName = "Otp";
        public string Pepper { get; set; } = default!;
        public int CodeMinutes { get; set; } = 5;
        public int MaxAttempts { get; set; } = 5;
        public int ResendCooldownSeconds { get; set; } = 60;
        public int MaxSendsPerHour { get; set; } = 5;
    }
}
