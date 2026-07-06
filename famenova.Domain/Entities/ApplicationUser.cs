using famenova.Domain.Common;
using famenova.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Entities
{
    public class ApplicationUser: IdentityUser<int>
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public DateTime? CreatedAccount { get; set; }


        public string? Address { get; set; }
        public string? Town { get; set; } 

        public AccountStatus Status { get; set; }

        //Relations 
        public Cart? cart { get; set; }

        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();
        //Token Filed
        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiry { get; set; }

        //OTP Field 
        public string? OtpCode { get; set; }
        public DateTime? ExpiryOtp { get; set; }



    }
}
