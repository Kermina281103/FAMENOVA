using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Otp
{
    public class EmailSettings
    {
        public const string SectionName = "EmailSettings";

        [Required, EmailAddress] public string SenderEmail { get; set; } = default!;
        public string SenderName { get; set; } = "Famenova";
        [Required] public string Password { get; set; } = default!;
        [Required] public string Server { get; set; } = default!;
        [Range(1, 65535)] public int Port { get; set; } = 587;
    }
}
