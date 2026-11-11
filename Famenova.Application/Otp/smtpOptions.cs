using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Otp
{
    public class SmtpOptions
    {
        public const string SectionName = "Smtp";

        public string Host { get; set; } = default!;
        public int Port { get; set; } = 587;
        public bool UseStartTls { get; set; } = true;
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string FromAddress { get; set; } = default!;
        public string FromName { get; set; } = "Famenova";
    }
}
