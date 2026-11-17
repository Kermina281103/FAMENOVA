using famenova.Domain.Constants;
using famenova.Domain.Entities;
using famenova.Domain.Enums;
using famenova.Domain.Interfaces;
using Famenova.Application.Common.Security;
using Famenova.Application.Interfaces;
using Famenova.Application.Jwt;
using Famenova.Shared;
using Famenova.Shared.Dtos.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using UnauthorizedException = Famenova.Application.Interfaces.UnauthorizedException;

namespace Famenova.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenGenerator _tokenGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtOptions _jwt;
        private readonly IOtpService _otpService;
        public AuthService(UserManager<ApplicationUser> userManager,IJwtTokenGenerator tokeGeneartor,IUnitOfWork unitOfWork,IOtpService otpService, IOptions<JwtOptions> jwtOptions)
        {
            _userManager = userManager;
            _tokenGenerator = tokeGeneartor;
            _unitOfWork = unitOfWork;
            _jwt = jwtOptions.Value;
            _otpService = otpService;

            
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) is not null)
                throw new ConflictException("This email is already registered");

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join("|", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, AppRoles.Customer);
            await _otpService.SendAsync(user.Id, user.Email!);
            return "OTP sent successfully. Please verify your email.";
        }

        public async  Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            const string invalid = "Invalid email or password";

            if (user is null)
                throw new UnauthorizedException(invalid);

            if (await _userManager.IsLockedOutAsync(user))
                throw new UnauthorizedException("Account is locked");
            if(!await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                await _userManager.AccessFailedAsync(user);
                throw new UnauthorizedException(invalid);
            }
            await _userManager.ResetAccessFailedCountAsync(user);
            if (!user.EmailConfirmed)
                throw new ForbiddenException("Please verify your email first.");
            return await BuildResponseAsync(user);

        }
       
        public async Task<AuthResponseDto> RefreshAsync(string refreshToken)
        {
            var hash = RefreshTokenHelper.Hash(refreshToken);
            var stored = (await _unitOfWork.GetGeneric<RefreshToken>()
                .GetAllAsync(t => t.TokenHash == hash)).FirstOrDefault()
                ?? throw new UnauthorizedException("Invalid Refresh Token");

            if(stored.RevokedAtUtc is not null)
            {
                await RevokeAllForUserAsync(stored.UserId);
                throw new UnauthorizedException("Session is no longer valid. Please log in again.");

            }
            if (stored.ExpiresAtUtc <= DateTime.UtcNow)
                throw new UnauthorizedException("session expired");
            var user = await _userManager.FindByIdAsync(stored.UserId.ToString())
    ?? throw new UnauthorizedException("Invalid Refresh Token");

            if (await _userManager.IsLockedOutAsync(user))
                throw new UnauthorizedException("Account is lockedout");
            return await BuildResponseAsync(user, stored);
        }
        public async Task LogoutAsync(string refreshToken)
        {
            var hash = RefreshTokenHelper.Hash(refreshToken);
            var stored = (await _unitOfWork.GetGeneric<RefreshToken>()
                .GetAllAsync(t => t.TokenHash == hash)).FirstOrDefault();

            if(stored is not null && stored.RevokedAtUtc is null)
            {
                stored.Revoke();
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task RevokeAllForUserAsync(int userId)
        {
            var active = await _unitOfWork.GetGeneric<RefreshToken>()
                .GetAllAsync(t => t.UserId == userId && t.RevokedAtUtc == null);

            foreach (var t in active) t.Revoke();
            await _unitOfWork.SaveChangesAsync();

        }
        public async Task ResetPasswordAsync(ResetPasswordDto request)
        {
            var errors = new List<string>();
            foreach (var validator in _userManager.PasswordValidators)
            {
                var check = await validator.ValidateAsync(_userManager, new ApplicationUser(), request.NewPassword);
                if (!check.Succeeded)
                    errors.AddRange(check.Errors.Select(e => e.Description));
            }
            if (errors.Count > 0)
                throw new BadRequestException(string.Join(" | ", errors));

            // 2) اليوزر
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null || !user.EmailConfirmed)
                throw new BadRequestException("Invalid or expired code.");

            await _otpService.VerifyAsync(user.Id, request.OtpCode);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(" | ", result.Errors.Select(e => e.Description)));

            await RevokeAllForUserAsync(user.Id);
        }
        public async Task ForgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

           
            if (user is null || user.Status != AccountStatus.Active)
                return;

            await _otpService.SendAsync(user.Id, email);
        }


        private async Task<AuthResponseDto> BuildResponseAsync(ApplicationUser user, RefreshToken? tokenToRotate = null)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var access = _tokenGenerator.Create(user, roles);

            var rawRefresh = RefreshTokenHelper.Generate();
            var newHash = RefreshTokenHelper.Hash(rawRefresh);
            var refresh = RefreshToken.Create(user.Id, newHash, DateTime.UtcNow.AddDays(_jwt.RefreshTokenDays));

            tokenToRotate?.Revoke(newHash);                             // old token: used up
            await _unitOfWork.GetGeneric<RefreshToken>().AddAsync(refresh);
            await _unitOfWork.SaveChangesAsync();                       // revoke old + save new, together

            return new AuthResponseDto
            {
                AccessToken = access.value,
                ExpiresAtUtc = access.ExpiresAtUtc,
                RefreshToken = rawRefresh,                              // the raw value goes to the client once
                RefreshTokenExpiresAtUtc = refresh.ExpiresAtUtc,
                UserId = user.Id,
                Email = user.Email!,
                FullName = $"{user.FirstName} {user.LastName}",
                Roles = roles
            };
        }

        
    }
}
