using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace Core.Common
{
    /// <summary>
    /// Provides utility methods for uploading and deleting files in AWS S3.
    /// </summary>
    /// <author>ThangTM</author>
    public class S3BucketAWS
    {
        private readonly IConfiguration _configuration;

        public S3BucketAWS(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Uploads a file asynchronously to the specified prefix path in AWS S3 bucket.
        /// </summary>
        /// <param name="file">The file to upload.</param>
        /// <param name="prefix">The S3 folder prefix (e.g., "customers/", "posts/").</param>
        /// <returns>The S3 key of the uploaded file.</returns>
        public async Task<string> UploadFileAsync(IFormFile file, string prefix, string fileName)
        {
            var accessKey = _configuration["AWS:AccessKey"];
            var secretKey = _configuration["AWS:SecretKey"];
            var regionName = _configuration["AWS:Region"];
            var bucketName = _configuration["AWS:BucketName"];

            var bucketRegion = RegionEndpoint.GetBySystemName(regionName);
            var s3Client = new AmazonS3Client(accessKey, secretKey, bucketRegion);

            var s3Key = $"{prefix}{fileName}";

            using (var stream = file.OpenReadStream())
            {
                var transferUtility = new TransferUtility(s3Client);
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = stream,
                    Key = s3Key,
                    BucketName = bucketName,
                    ContentType = file.ContentType
                };
                await transferUtility.UploadAsync(uploadRequest);
            }

            return s3Key;
        }

        /// <summary>
        /// Deletes a file from the AWS S3 bucket by its key (for images: thumb_ and full_).
        /// </summary>
        /// <param name="folder">The name of folder of the file to delete.</param>
        /// <param name="fileNameWithoutPrefix">The filename (path) of the file to delete.</param>
        public async Task DeleteFileAsync(string folder, string fileNameWithoutPrefix)
        {
            var accessKey = _configuration["AWS:AccessKey"];
            var secretKey = _configuration["AWS:SecretKey"];
            var regionName = _configuration["AWS:Region"];
            var bucketName = _configuration["AWS:BucketName"];

            var bucketRegion = RegionEndpoint.GetBySystemName(regionName);
            var s3Client = new AmazonS3Client(accessKey, secretKey, bucketRegion);

            // Tạo key cho cả 2 file thumb_ và full_ (dành cho ảnh)
            var thumbKey = $"{folder}/thumb_{fileNameWithoutPrefix}";
            var fullKey = $"{folder}/full_{fileNameWithoutPrefix}";

            try
            {
                // Xóa thumb_
                await s3Client.DeleteObjectAsync(new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = thumbKey
                });

                // Xóa full_
                await s3Client.DeleteObjectAsync(new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = fullKey
                });
            }
            catch (AmazonS3Exception ex)
            {
                // Log lỗi nếu cần (ví dụ: file không tồn tại), nhưng không throw để an toàn
                Console.WriteLine($"S3 Delete Error for image {thumbKey}/{fullKey}: {ex.Message}");
                // Không throw ở đây, vì delete idempotent (an toàn nếu file đã xóa)
            }
            catch (Exception ex)
            {
                // Các lỗi khác (network, permission) → throw để handle ở caller
                throw new Exception($"Lỗi xóa ảnh S3: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Deletes a video file from the AWS S3 bucket (single file, no thumb/full).
        /// </summary>
        /// <param name="folder">The folder path (e.g., "posts/videos").</param>
        /// <param name="videoFileName">The exact filename of the video (e.g., "video123.mp4").</param>
        public async Task DeleteVideoFileAsync(string folder, string videoFileName)
        {
            var accessKey = _configuration["AWS:AccessKey"];
            var secretKey = _configuration["AWS:SecretKey"];
            var regionName = _configuration["AWS:Region"];
            var bucketName = _configuration["AWS:BucketName"];

            var bucketRegion = RegionEndpoint.GetBySystemName(regionName);
            var s3Client = new AmazonS3Client(accessKey, secretKey, bucketRegion);

            var videoKey = $"{folder}/{videoFileName}";

            try
            {
                await s3Client.DeleteObjectAsync(new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = videoKey
                });
                Console.WriteLine($"✅ Deleted video from S3: {videoKey}");  // Log debug (xóa sau khi test)
            }
            catch (AmazonS3Exception ex)
            {
                // Nếu file không tồn tại (404 NoSuchKey), chỉ log và không throw (idempotent)
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"⚠️ Video không tồn tại trên S3 (đã xóa trước): {videoKey}");
                }
                else
                {
                    // Các lỗi S3 khác (permission, etc.) → log và throw
                    Console.WriteLine($"❌ S3 Delete Error for video {videoKey}: {ex.Message}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                // Các lỗi khác (network, etc.) → throw để handle ở caller
                Console.WriteLine($"❌ General error deleting video {videoKey}: {ex.Message}");
                throw new Exception($"Lỗi xóa video S3: {ex.Message}", ex);
            }
        }
    }
}
