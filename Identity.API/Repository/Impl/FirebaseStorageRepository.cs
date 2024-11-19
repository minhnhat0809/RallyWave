using System.Web;
using FirebaseAdmin;
using Google;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Identity.API.Ultility;

namespace Identity.API.Repository.Impl;

public interface IFirebaseStorageRepository
{
    Task<string> UploadImageAsync(string name, IFormFile file, string imgFolderName);
    Task<bool> DeleteImageByUrlAsync(string fileUrl);
}
public class FirebaseStorageRepository : IFirebaseStorageRepository
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;

        public FirebaseStorageRepository(IConfiguration configuration)
        {
            // Initialize Firebase credentials and storage client
            var secret = new GetSecret();
            var credential = GoogleCredential.FromJson(secret.GetFireBaseCredentials().Result);
            _storageClient = StorageClient.Create(credential);

            // Set the bucket name from configuration
            _bucketName = configuration["Authentication:Firebase:StorageBucket"] ?? throw new InvalidOperationException();
        }
        
        public async Task<string> UploadImageAsync(string name, IFormFile file, string imgFolderName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File Is Null or Empty.");

            // Kiểm tra ContentType, chỉ chấp nhận các loại file ảnh
            var allowedContentTypes = new List<string> { "image/jpeg", "image/png", "image/jpg", "image/gif" };
            if (!allowedContentTypes.Contains(file.ContentType.ToLower()))
            {
                throw new ArgumentException("Only accepted image file: (jpeg, jpg, png, gif).");
            }

            // Lấy đuôi file
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            var fileName = $"{imgFolderName}/{name}{fileExtension}";

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            // Tạo một token ngẫu nhiên để tạo link public
            var token = Guid.NewGuid().ToString();

            // Thêm metadata để có thể public file với token
            var metadata = new Google.Apis.Storage.v1.Data.Object
            {
                Bucket = _bucketName,
                Name = fileName,
                ContentType = file.ContentType,
                Metadata = new Dictionary<string, string>
                {
                    { "firebaseStorageDownloadTokens", token }
                }
            };

            // Upload file với quyền PublicRead
            await _storageClient.UploadObjectAsync(metadata, stream, new UploadObjectOptions
            {
                PredefinedAcl = PredefinedObjectAcl.PublicRead
            });

            // Trả về URL công khai với token
            return GetFirebaseMediaUrl(fileName, token);
        }
        private string GetFirebaseMediaUrl(string fileName, string token)
        {
            return $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/{Uri.EscapeDataString(fileName)}?alt=media&token={token}";
        }
        public async Task<bool> DeleteImageByUrlAsync(string fileUrl)
        {
            try
            {
                // Parse the URL to extract the bucket and object name
                var uri = new Uri(fileUrl);
        
                // Get the bucket name from your configuration
                string bucketName = _bucketName; // Ensure _bucketName is already set up in your code
        
                // Extract the object name from the URL
                string objectName = HttpUtility.UrlDecode(uri.AbsolutePath.Replace($"/v0/b/{bucketName}/o/", ""));
        
                // Validate if objectName is valid
                if (string.IsNullOrEmpty(objectName))
                {
                    throw new ArgumentException("The file name could not be found in the URL.");
                }

                // Delete the object from Firebase Storage
                await _storageClient.DeleteObjectAsync(bucketName, objectName);
                return true;
            }
            catch (Google.GoogleApiException e) when (e.Error.Code == 404)
            {
                Console.WriteLine($"File not found: {e.Message}");
                return false; // The file does not exist
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting the image: {ex.Message}");
                throw;
            }
        }

    }