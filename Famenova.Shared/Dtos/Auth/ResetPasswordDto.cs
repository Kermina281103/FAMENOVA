using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared.Dtos.Auth
{
    public class ResetPasswordDto
    {
        public string Email { get; set; } = default!;

        public string Token { get; set; } = default!;

        public string NewPassword { get; set; } = default!;

        public string OtpCode { get; set; }
    }
}
