using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<string> SaveImageAsync(IFormFile image, string folderName)
        {
            var folderPath = Path.Combine(_environment.WebRootPath, "Images", folderName);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}" + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(folderPath, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await image.CopyToAsync(stream);
            return $"/Images/{folderName}/{fileName}";
        }

    }
}
