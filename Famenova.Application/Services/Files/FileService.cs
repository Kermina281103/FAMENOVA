using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famenova.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Famenova.Application.Services.Files
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
      
       public FileService(IWebHostEnvironment environment)
       {
           _environment = environment;
       }

        public void DeleteImage(string? ImagePath)
        {
            if (string.IsNullOrEmpty(ImagePath)) return;
            var fullPath = Path.Combine(_environment.WebRootPath, ImagePath.TrimStart('/', '\\'));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

        private static readonly Dictionary<string, string[]> Allowed = new(StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"] = new[] { "image/jpeg" },
            [".jpeg"] = new[] { "image/jpeg" },
            [".png"] = new[] { "image/png" },
            [".webp"] = new[] { "image/webp" },
            [".pdf"] = new[] { "application/pdf" },
        };
        private const long MaxBytes = 5 * 1024 * 1024;

        public async Task<string> SaveImageAsync(IFormFile image, string folderName, bool allowPdf = false)
        {
            if (image is null || image.Length == 0)
                throw new BadRequestException("File is required.");
            if (image.Length > MaxBytes)
                throw new BadRequestException("File must be 5 MB or smaller.");

            var ext = Path.GetExtension(image.FileName);
            if (!Allowed.TryGetValue(ext, out var types) ||
                !types.Contains(image.ContentType, StringComparer.OrdinalIgnoreCase) ||
                (ext.Equals(".pdf", StringComparison.OrdinalIgnoreCase) && !allowPdf))
                throw new BadRequestException(allowPdf
                    ? "Only JPG, PNG, WebP or PDF files are allowed."
                    : "Only JPG, PNG or WebP images are allowed.");

            var header = new byte[12];
            int read;
            await using (var probe = image.OpenReadStream())
                read = await probe.ReadAtLeastAsync(header, header.Length, throwOnEndOfStream: false);
            if (!HasValidSignature(header, read, ext))
                throw new BadRequestException("The file content does not match its type.");

            var folderPath = Path.Combine(_environment.WebRootPath, "Images", folderName);
            Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid():N}{ext.ToLowerInvariant()}";   
            await using var stream = new FileStream(Path.Combine(folderPath, fileName), FileMode.CreateNew);
            await image.CopyToAsync(stream);

            return $"/Images/{folderName}/{fileName}";
        }

        private static bool HasValidSignature(byte[] h, int length, string ext) =>
            ext.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => length >= 3 && h[0] == 0xFF && h[1] == 0xD8 && h[2] == 0xFF,
                ".png" => length >= 8 && h[0] == 0x89 && h[1] == 0x50 && h[2] == 0x4E && h[3] == 0x47
                                       && h[4] == 0x0D && h[5] == 0x0A && h[6] == 0x1A && h[7] == 0x0A,
                ".webp" => length >= 12 && h[0] == 'R' && h[1] == 'I' && h[2] == 'F' && h[3] == 'F'
                                        && h[8] == 'W' && h[9] == 'E' && h[10] == 'B' && h[11] == 'P',
                ".pdf" => length >= 4 && h[0] == '%' && h[1] == 'P' && h[2] == 'D' && h[3] == 'F',
                _ => false
            };

    }
}
