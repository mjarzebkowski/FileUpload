using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace FileUpload.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IHostEnvironment _environment;

        public FileUploadController(IHostEnvironment environment)
        {
            this._environment = environment;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm]IFormFile file)
        {
            if(file == null || file.Length == 0)
                return BadRequest("No file or zero-lenght file");

            string fileName = file.FileName;
            string extension = Path.GetExtension(fileName);

            string[] allowedExtensions = { ".jpg", ".png", ".gif", ".bmp" };

            if (!allowedExtensions.Contains(extension))
                return BadRequest("File do not fit correct extensions");

            string newFileName = $"{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(_environment.ContentRootPath, "wwwroot", "Uploads", newFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(fileStream);
            }

            return Ok($"Uploads/{newFileName}");
        }
    }
}
