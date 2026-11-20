using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Services.Files
{
    public interface IFileService
    {
        Task<string> SaveImageAsync(IFormFile image, string folderName,bool allowPdf=false);
        void DeleteImage(string? ImagePath);

    }
}
