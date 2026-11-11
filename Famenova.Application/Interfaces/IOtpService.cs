using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Interfaces
{
    public interface IOtpService
    {
        Task SendAsync(int userId, string email);
        Task VerifyAsync(int userId, string code);
    }
}
