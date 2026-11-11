using Famenova.Shared.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> RefreshAsync(string refreshToken);
        Task LogoutAsync(string refreshToken);
        Task RevokeAllForUserAsync(int userId);
        Task<string> ResetPasswordAsync(ResetPasswordDto request);
        Task<string> ForgetPasswordAsync(string email);
        
        }
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message)
        {

        }
    }
}
