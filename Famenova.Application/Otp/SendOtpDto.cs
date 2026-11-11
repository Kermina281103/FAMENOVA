using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Otp
{
    public class SendOtpDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = default!;
    }

    public class VerifyOtpDto
    {
        [Required, EmailAddress] 
        public string Email { get; set; } = default!;

        [Required, RegularExpression(@"^\d{6}$", ErrorMessage = "Code must be 6 digits")]
        public string Code { get; set; } = default!;
    }
}
