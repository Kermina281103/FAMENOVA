using famenova.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Interfaces
{
    
    public interface IOtpSender
    {
        Task SendAsync(string destination, string code);
    }
}
