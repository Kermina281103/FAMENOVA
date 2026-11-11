using famenova.Domain.Entities;
using famenova.Domain.Interfaces;
using Famenova.Application.Exceptions;
using Famenova.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Services
{
    public class OtpService : IOtpService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOtpSender _sender;
        private readonly string _pepper;
        public OtpService(IUnitOfWork unitOfWork,IOtpSender sender,IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _sender = sender;
            _pepper = configuration["Otp:Pepper"] ??
                throw new ConflictException("Otp:Pepper is missing from settings");
        }

      

        public async Task SendAsync(int userId, string email)
        {
            var repo = _unitOfWork.GetGeneric<OtpCode>();

            var old = (await repo.GetAllAsync(o => o.UserId == userId && !o.IsUsed)).ToList();

            if (old.Any(o => o.CreatedAtUtc > DateTime.UtcNow.AddSeconds(-60)))
            {
                throw new BadRequestException("Please wait a minute before sending a new ");
            }
            foreach (var o in old) o.IsUsed = true;
            var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
            await repo.AddAsync(new OtpCode
            {
                UserId = userId,
                CodeHash = Hash(userId, code.ToString()),
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(5)

            });
            await _unitOfWork.SaveChangesAsync();
            await _sender.SendAsync(email, code); 
        }

        public async Task VerifyAsync(int userId, string code)
        {
            var otp = (await _unitOfWork.GetGeneric<OtpCode>()
                 .GetAllAsync(o => o.UserId == userId && !o.IsUsed))
                 .OrderByDescending(o => o.CreatedAtUtc)
                 .FirstOrDefault();

            if (otp is null || otp.ExpiresAtUtc < DateTime.UtcNow | otp.FailedAttempts >= 5)
                throw new BadRequestException("Invalid Otp");

            if (otp.CodeHash != Hash(userId, code))
            {
                otp.FailedAttempts++;
                await _unitOfWork.SaveChangesAsync();
                throw new BadRequestException("Invalid Otp ");
            }

            otp.IsUsed = true;
            await _unitOfWork.SaveChangesAsync();
                
        }

        private string Hash(int userId, string code)
        {
          return (Convert.ToHexString(HMACSHA256.HashData(
            Encoding.UTF8.GetBytes(_pepper),
            Encoding.UTF8.GetBytes($"{userId}:{code}"))));
        }
    }
}
