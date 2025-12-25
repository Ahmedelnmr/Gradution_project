using Homy.Application.Contract_Service.ApiServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Homy.Application.Service.ApiServices
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        public FileUploadService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public (bool isValid, string errorMessage) ValidateImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return (false, "الملف فارغ");

            // Check file size
            if (file.Length > _maxFileSize)
                return (false, $"حجم الملف يتجاوز الحد المسموح ({_maxFileSize / 1024 / 1024}MB)");

            // Check file extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                return (false, $"نوع الملف غير مسموح. الأنواع المسموحة: {string.Join(", ", _allowedExtensions)}");

            // Check content type
            var allowedContentTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
            if (!allowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
                return (false, "نوع المحتوى غير صحيح");

            return (true, string.Empty);
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder)
        {
            // Validate image
            var (isValid, errorMessage) = ValidateImage(file);
            if (!isValid)
                throw new InvalidOperationException(errorMessage);

            // Create upload directory if not exists
            var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", folder);
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // Generate unique filename
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return relative URL
            return $"/uploads/{folder}/{fileName}";
        }

        public async Task<List<string>> UploadImagesAsync(List<IFormFile> files, string folder)
        {
            var uploadedUrls = new List<string>();

            foreach (var file in files)
            {
                try
                {
                    var url = await UploadImageAsync(file, folder);
                    uploadedUrls.Add(url);
                }
                catch (Exception ex)
                {
                    // Log error (you can inject ILogger here)
                    Console.WriteLine($"Error uploading file {file.FileName}: {ex.Message}");
                    // Continue with other files
                }
            }

            return uploadedUrls;
        }

        public Task<bool> DeleteFileAsync(string fileUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(fileUrl))
                    return Task.FromResult(false);

                // Extract file path from URL
                var fileName = fileUrl.Replace("/uploads/", "");
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file {fileUrl}: {ex.Message}");
                return Task.FromResult(false);
            }
        }
    }
}
