using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Shared.Storage;
public interface IFileStorageService
{
    Task<ImageUploadResult> UploadFileAsync(IFormFile file, string folder);
    Task<DeletionResult> DeleteFileAsync(string publicId);
    Task<ImageUploadResult> UpdateFileAsync(IFormFile newFile, string existingPath, string folder);
}