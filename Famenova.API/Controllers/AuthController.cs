using Famenova.Application.Exceptions;
using Famenova.Application.Interfaces;
using Famenova.Application.Responses;
using Famenova.Shared.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace Famenova.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController:ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        [AllowAnonymous]
        
        public async Task<ActionResult<ApiResponse<string>>> Register([FromForm]RegisterDto dto)
        {
            await _authService.RegisterAsync(dto);

            return ( Ok(new { message = "Registered. Check your email for the verification code." }));

        }

        [HttpPost("login")]
        [AllowAnonymous]

        public async Task<ActionResult<ApiResponse<AuthResponseDto>>>Login([FromForm]LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(ApiResponse<AuthResponseDto>.Success(result, "User Login Successfully"));

        }

        [HttpGet("me")]
        [Authorize]
        public ActionResult<object> Me() => Ok(new
        {
            userId = User.GetUserId(),
            email = User.FindFirstValue(ClaimTypes.Email),
            roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value)
        });


        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Refresh(RefreshRequestDto dto)
    => Ok(ApiResponse<AuthResponseDto>.Success(await _authService.RefreshAsync(dto.RefreshToken)));

        [HttpPost("logout")]
        [AllowAnonymous]   // the refresh token is the proof, and the access token may already be expired
        public async Task<IActionResult> Logout(RefreshRequestDto dto)
        {
            await _authService.LogoutAsync(dto.RefreshToken);
            return Ok();
        }

        [HttpPost("logout-all")]
        [Authorize]
        public async Task<IActionResult> LogoutAll()
        {
            await _authService.RevokeAllForUserAsync(User.GetUserId());
            return Ok();
        }

        [HttpPost("forget-password")]
        public async Task<ActionResult> ForgetPassword([FromForm] string email)
        {
            await _authService.ForgetPasswordAsync(email);
            return Ok(new { message = "If the account exists, a code was sent." });
        }
        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto request)
        {
             await _authService.ResetPasswordAsync(request);
            return Ok(new { message = "Reset Password Done successfully " });
        }

    }
}
public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
        => int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User id claim is missing."));
}
