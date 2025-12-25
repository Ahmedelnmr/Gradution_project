using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Homy.Application.Contract_Service.ApiServices
{
    /// <summary>
    /// Service for handling file uploads (images, documents, etc.)
    /// </summary>
    public interface IFileUploadService
    {
        /// <summary>
        /// Upload a single image
        /// </summary>
        /// <param name="file">Image file</param>
        /// <param name="folder">Target folder (e.g., "properties", "profiles")</param>
        /// <returns>Uploaded file URL</returns>
        Task<string> UploadImageAsync(IFormFile file, string folder);

        /// <summary>
        /// Upload multiple images
        /// </summary>
        /// <param name="files">Image files</param>
        /// <param name="folder">Target folder</param>
        /// <returns>List of uploaded file URLs</returns>
        Task<List<string>> UploadImagesAsync(List<IFormFile> files, string folder);

        /// <summary>
        /// Delete a file by URL
        /// </summary>
        /// <param name="fileUrl">File URL to delete</param>
        /// <returns>True if deleted successfully</returns>
        Task<bool> DeleteFileAsync(string fileUrl);

        /// <summary>
        /// Validate image file (size, format)
        /// </summary>
        /// <param name="file">File to validate</param>
        /// <returns>Validation result (success, errorMessage)</returns>
        (bool isValid, string errorMessage) ValidateImage(IFormFile file);
    }
}
