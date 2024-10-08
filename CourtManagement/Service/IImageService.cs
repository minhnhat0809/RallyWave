using CourtManagement.DTOs;

namespace CourtManagement.Service;

public interface IImageService
{
    Task<bool> CreateBucket(string bucketName);

    Task<List<string>> GetAllBuckets();

    Task DeleteBucket(string bucketName);

    Task<List<string>> UploadImages(IFormFileCollection images, string bucketName, string? prefix);

    Task DeleteImage(string bucketName, string key);
}