using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Shared.Storage.Cloudinary;
public class CloudinaryFileStorageService : IFileStorageService
{
    private readonly CloudinaryDotNet.Cloudinary _cloudinary;
    private readonly string _serviceName;
    private readonly ILogger<CloudinaryFileStorageService> _logger;
    private readonly CloudinarySettings _settings;

    public CloudinaryFileStorageService(IOptions<CloudinarySettings> opts, ILogger<CloudinaryFileStorageService> logger, string serviceName)
    {
        _settings = opts.Value;
        _serviceName = serviceName;
        _logger = logger;

        var account = new Account(_settings.CloudName, _settings.ApiKey, _settings.ApiSecret);
        _cloudinary = new CloudinaryDotNet.Cloudinary(account);
        _cloudinary.Api.Secure = true;
    }

    public async Task<ImageUploadResult> UploadFileAsync(IFormFile file, string folderPath)
    {
        try
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.Name, file.OpenReadStream()),
                Folder = GetFolderPathName(folderPath),
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult;

        }
        catch (Exception ex)
        {
            _logger.LogError("An error has occurred during attempt to save file message: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<ImageUploadResult> UpdateFileAsync(IFormFile file, string folderPath, string publicId)
    {
        try
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.Name, stream),
                Folder = GetFolderPathName(folderPath),
                PublicId = publicId,
                Overwrite = true,
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error has occurred during attempt to save file message: {Message}", ex.Message);
            throw;
        }
    }

    public async Task<DeletionResult> DeleteFileAsync(string publicId)
    {
        try
        {
            var deleteParams = new DeletionParams(GetFolderPathName(publicId));
            var destoryResult = await _cloudinary.DestroyAsync(deleteParams);
            return destoryResult;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error has occurred during attempt to save file message: {Message}", ex.Message);
            throw;
        }
    }

    private string GetFolderPathName(string folderPath)
    {
        return $"{_serviceName}/{folderPath}";
    }
}