using Famenova.Application.Exceptions;
using Famenova.Application.Interfaces;
using famenova.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Famenova.Application.Otp;

namespace Famenova.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OtpController(UserManager<ApplicationUser> _userManager, IOtpService _otpService) : ControllerBase
    {
        [HttpPost("send")]
        [AllowAnonymous]
        public async Task<IActionResult> Send(SendOtpDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is not null && !user.EmailConfirmed)
                await _otpService.SendAsync(user.Id, user.Email!);

            return Ok(new { message = "If the account exists, a code was sent." });
        }

        [HttpPost("verify")]
        [AllowAnonymous]
        public async Task<IActionResult> Verify(VerifyOtpDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email)
                ?? throw new BadRequestException("Invalid or expired code.");

            await _otpService.VerifyAsync(user.Id, dto.Code);

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            return Ok(new { message = "Email verified successfully." });
        }
    }
}
