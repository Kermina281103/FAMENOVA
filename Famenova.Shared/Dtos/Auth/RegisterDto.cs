using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Shared.Dtos.Auth
{
   public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;
        [Required, MinLength(8)]
        public string Password { get; set; } = default!;
        [Required]
        public string FirstName { get; set; } = default!;
        [Required]
        public string LastName { get; set; } = default!;
        public string? PhoneNumber { get; set; }
    }
}
