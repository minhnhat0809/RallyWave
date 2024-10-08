using Amazon.S3;
using Amazon.S3.Model;

namespace CourtManagement.Service.Impl;

public class ImageService(IAmazonS3 s3Client) : IImageService
{
    private readonly IAmazonS3 _s3Client = s3Client;
    
    public async Task<bool> CreateBucket(string bucketName)
    {
        var bucketExist = await Amazon.S3.Util.AmazonS3Util.
            DoesS3BucketExistV2Async(_s3Client, bucketName);

        if (bucketExist) return false;

        await _s3Client.PutBucketAsync(bucketName);
        return true;
    }

    public async Task<List<string>> GetAllBuckets()
    {
        var data = await _s3Client.ListBucketsAsync();

        var bucketList = data.Buckets.Select(b => b.BucketName.ToString()).ToList();

        return bucketList;
    }

    public async Task DeleteBucket(string bucketName)
    {
        await _s3Client.DeleteBucketAsync(bucketName);
    }

    public async Task<List<string>> UploadImages(IFormFileCollection images, string bucketName, string? prefix)
    {
        try
        {
            var imageUrlList = new List<string>();

            // Check if the bucket exists
            var bucketExist = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            if (!bucketExist) throw new Exception("There are no buckets with this name");

            foreach (var image in images)
            {
                // Generate a unique key for each image
                var fileName = $"{Guid.NewGuid()}_{image.FileName}";
                var s3Key = string.IsNullOrEmpty(prefix) ? fileName : $"{prefix.TrimEnd('/')}/{fileName}";

                // Create the PutObjectRequest
                var request = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = s3Key,
                    InputStream = image.OpenReadStream(),
                    ContentType = image.ContentType
                };

                // Upload the image to S3
                await _s3Client.PutObjectAsync(request);

                // Add the URL to the list
                var imageUrl = $"https://{bucketName}.s3.amazonaws.com/{s3Key}";
                imageUrlList.Add(imageUrl);
            }

            return imageUrlList;
        }
        catch (AmazonS3Exception e)
        {
            throw new AmazonS3Exception(e.Message);
        }
    }

    public async Task DeleteImage(string bucketName, string key)
    {
        try
        {
            // Check if the bucket exists
            var bucketExist = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            if (!bucketExist) throw new Exception("There are no buckets with this name");
            
            //define delete object
            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(deleteObjectRequest);
        }
        catch (AmazonS3Exception e)
        {
            throw new AmazonS3Exception(e.Message);
        }
    }
}