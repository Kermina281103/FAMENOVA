using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace famenova.Domain.Enums
{
    public enum OtpPurpose { EmailVerification = 1, PhoneVerification = 2, PasswordReset = 3 }
    public enum OtpChannel { Email = 1, Sms = 2 }

}
