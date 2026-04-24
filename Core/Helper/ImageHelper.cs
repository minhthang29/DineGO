using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Core.Common;

public class ImageHelper
{
    private readonly S3BucketAWS _s3Bucket;
    public ImageHelper(S3BucketAWS s3Bucket)
    {
        _s3Bucket = s3Bucket;
    }

    public Stream ResizeImage(Stream inputStream, int maxWidth)
    {
        using var originalImage = Image.FromStream(inputStream);

        if (originalImage.Width <= maxWidth)
        {
            var copyStream = new MemoryStream();
            inputStream.Position = 0;
            inputStream.CopyTo(copyStream);
            copyStream.Position = 0;
            return copyStream;
        }

        double ratio = (double)maxWidth / originalImage.Width;
        int newWidth = maxWidth;
        int newHeight = (int)(originalImage.Height * ratio);

        var resized = new Bitmap(newWidth, newHeight);

        using (var graphics = Graphics.FromImage(resized))
        {
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);
        }

        var outputStream = new MemoryStream();
        resized.Save(outputStream, ImageFormat.Jpeg);
        outputStream.Position = 0;
        return outputStream;
    }

    public IFormFile ResizeToFormFile(IFormFile original, int maxWidth, string newFileName)
    {
        var resizedStream = ResizeImage(original.OpenReadStream(), maxWidth);
        return new FormFile(resizedStream, 0, resizedStream.Length, "thumbnail", newFileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };
    }

    public async Task<string> UploadImageWithThumbnailAsync(
     IFormFile image,
     string folderName,
     int thumbWidth
 )
    {
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";

        // 📌 Đọc kích thước ảnh gốc
        using var imageStream = image.OpenReadStream();
        using var tempImage = Image.FromStream(imageStream);

        // ⚠️ Reset stream để dùng lại
        imageStream.Position = 0;

        if (tempImage.Width > 1920)
        {
            // Nếu ảnh lớn hơn 1920px thì resize full
            var resizedFullStream = ResizeImage(imageStream, 1920);
            var fullFormFile = new FormFile(resizedFullStream, 0, resizedFullStream.Length, "full", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
            await _s3Bucket.UploadFileAsync(fullFormFile, $"{folderName}/full_", fileName);
        }
        else
        {
            // Giữ nguyên nếu nhỏ hơn hoặc bằng 1920
            await _s3Bucket.UploadFileAsync(image, $"{folderName}/full_", fileName);
        }

        // 👇 Resize ảnh thumbnail và upload
        var thumbFormFile = ResizeToFormFile(image, thumbWidth, fileName);
        await _s3Bucket.UploadFileAsync(thumbFormFile, $"{folderName}/thumb_", fileName);

        return fileName;
    }
    public async Task<string> UploadPdfAsync(IFormFile file, string folderName)
    {
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        await _s3Bucket.UploadFileAsync(file, $"{folderName}/", fileName);
        return fileName;
    }
    public async Task<string> UploadVideoAsync(IFormFile video, string folderName)
    {
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(video.FileName)}";
        await _s3Bucket.UploadFileAsync(video, $"{folderName}/", fileName);
        return fileName;
    }

}
