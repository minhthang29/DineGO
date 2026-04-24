using Core.Common;
using Core.Constant;
using Core.Models;
using Core.Services;
using Core.Models.Client.Custom;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using System.Text.Json;

namespace Core.Services
{
    /// <summary>
    /// Service for handling all operations related to Posts.
    /// </summary>
    /// <author>ThangTM</author>
    public class PostService
    {
        private readonly ApiService _apiService;
        private readonly ImageHelper _imageHelper;
        private readonly S3BucketAWS _S3;

        public PostService(ApiService apiService, ImageHelper imageHelper, S3BucketAWS S3)
        {
            _apiService = apiService;
            _imageHelper = imageHelper;
            _S3 = S3;
        }

        /// <summary>
        /// Retrieves the list of all posts and related data.
        /// </summary>
        public async Task<CustomPostViewModel> GetAllAsync(bool showMine, int? cus_id)
        {
            var posts = await _apiService.GetAsync<List<Post>>(ApiEndpoints.POST);
            if (showMine)
            {
                posts = posts.Where(p => p.cus_id == cus_id).ToList();
            }
            var comments = await _apiService.GetAsync<List<Comment>>(ApiEndpoints.COMMENT);
            var customers = await _apiService.GetAsync<List<Customer>>(ApiEndpoints.CUSTOMER);
            var customer = await _apiService.GetAsync<Customer>($"{ApiEndpoints.CUSTOMER}/{cus_id}");
            var likes = await _apiService.GetAsync<List<Like>>(ApiEndpoints.LIKE);

            return new CustomPostViewModel
            {
                Posts = posts,
                Comments = comments,
                Customers = customers,
                Customer = customer,
                Likes = likes
            };
        }

        /// <summary>
        /// Creates a new post for a customer.
        /// </summary>
        public async Task CreatePostAsync(
            int cus_id,
            string post_content,
            List<IFormFile>? post_images,
            IFormFile? post_video)
        {
            // Kiểm tra số bài viết chờ duyệt (nếu >=3 thì không cho tạo mới)
            var posts = await _apiService.GetAsync<List<Post>>(ApiEndpoints.POST);
            var unapprovedCount = posts.Where(p => p.cus_id == cus_id && !p.post_is_approve).Count();
            if (unapprovedCount >= 3)
            {
                throw new ArgumentException("Bạn đã có 3 bài viết đang chờ duyệt. Vui lòng chờ duyệt trước khi đăng bài mới.");
            }

            // Validate ảnh theo (số lượng, type, size)
            if (post_images != null && post_images.Count > 0)
            {
                if (post_images.Count > 10)
                    throw new ArgumentException("Mỗi bài đăng chỉ được phép tối đa 10 ảnh.");

                var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png" };
                const long maxImageSize = 5 * 1024 * 1024;  // 5MB

                for (int i = 0; i < post_images.Count; i++)
                {
                    var image = post_images[i];
                    if (image == null || image.Length == 0)
                        throw new ArgumentException($"File không hợp lệ (rỗng).");

                    var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                    if (!allowedImageExtensions.Contains(extension))
                        throw new ArgumentException($"Chỉ hỗ trợ định dạng JPG, JPEG, PNG.");

                    if (image.Length > maxImageSize)
                        throw new ArgumentException($"Dung lượng vượt quá 5MB (hiện tại: {Math.Round((double)image.Length / 1024 / 1024, 1)}MB).");

                    // Optional: Check ContentType (thêm layer bảo vệ)
                    if (!image.ContentType.StartsWith("image/"))
                        throw new ArgumentException($"Loại file không phải ảnh (ContentType: {image.ContentType}).");
                }
            }

            // Validate video theo BR-40 (type, size)
            if (post_video != null && post_video.Length > 0)
            {
                var allowedVideoExtensions = new[] { ".mp4", ".mov" };
                const long maxVideoSize = 100 * 1024 * 1024;  // 100MB

                if (post_video.Length == 0)
                    throw new ArgumentException("File video không hợp lệ (rỗng).");

                var extension = Path.GetExtension(post_video.FileName).ToLowerInvariant();
                if (!allowedVideoExtensions.Contains(extension))
                    throw new ArgumentException($"Video: Chỉ hỗ trợ định dạng MP4, MOV.");

                if (post_video.Length > maxVideoSize)
                    throw new ArgumentException($"Video: Dung lượng vượt quá 100MB (hiện tại: {Math.Round((double)post_video.Length / 1024 / 1024, 1)}MB).");

                // Optional: Check ContentType
                if (!post_video.ContentType.StartsWith("video/"))
                    throw new ArgumentException($"Video: Loại file không phải video (ContentType: {post_video.ContentType}).");
            }

            var fileNames = new List<string>();
            string? videoName = null;

            // upload ảnh (chỉ nếu valid)
            if (post_images != null)
            {
                foreach (var image in post_images)
                {
                    var fileName = await _imageHelper
                        .UploadImageWithThumbnailAsync(image, "posts", thumbWidth: 600);
                    fileNames.Add(fileName);
                }
            }

            // upload video (chỉ nếu valid)
            if (post_video != null)
            {
                videoName = await _imageHelper
                    .UploadVideoAsync(post_video, "posts/videos");
            }

            var post = new Post
            {
                cus_id = cus_id,
                post_content = post_content,
                post_image = fileNames.Any() ? JsonSerializer.Serialize(fileNames) : null,
                post_video = videoName,
                post_created_date = DateTime.Now
            };

            await _apiService.PostAsync<object, Post>($"{ApiEndpoints.POST}", post);
        }

        /// <summary>
        /// Edits an existing post with optional image and video replacement.
        /// </summary>
        public async Task EditPostAsync(
            int cus_id,
            int post_id,
            string post_content,
            string old_images,
            List<IFormFile> post_images,
            IFormFile? post_video = null,  // 👈 THÊM: Video mới (optional)
            string? old_video = null)      // 👈 THÊM: Tên video cũ từ form (để biết giữ/xóa)
        {
            var existingPost = await _apiService.GetAsync<Post>($"{ApiEndpoints.POST}/id?ID={post_id}");
            if (existingPost == null || existingPost.cus_id != cus_id)
                throw new UnauthorizedAccessException();

            // 🔥 XỬ LÝ ẢNH (giữ nguyên logic cũ)
            var imagesToKeep = (old_images ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

            // So sánh với ảnh cũ → xoá ảnh bị bỏ
            var oldImageList = existingPost.post_image != null
                ? JsonSerializer.Deserialize<List<string>>(existingPost.post_image) ?? new List<string>()
                : new List<string>();
            foreach (var oldImg in oldImageList)
            {
                if (!imagesToKeep.Contains(oldImg))
                    await _S3.DeleteFileAsync("posts", oldImg);  // Xóa ảnh cũ (thumb_ + full_)
            }

            // 👈 THÊM: Validate ảnh mới theo BR-39 (type, size - tương tự Create)
            if (post_images != null && post_images.Count > 0)
            {
                // Kiểm tra tổng số ảnh không vượt quá 10 (giữ nguyên)
                var totalImages = imagesToKeep.Count + post_images.Count;
                if (totalImages > 10)
                {
                    throw new ArgumentException("Tổng số ảnh không được vượt quá 10 ảnh cho mỗi bài đăng.");
                }

                // Validate từng ảnh mới (extension, size, ContentType)
                var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png" };
                const long maxImageSize = 5 * 1024 * 1024;  // 5MB

                for (int i = 0; i < post_images.Count; i++)
                {
                    var image = post_images[i];
                    if (image == null || image.Length == 0)
                        throw new ArgumentException($"Ảnh mới: File không hợp lệ (rỗng).");

                    var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                    if (!allowedImageExtensions.Contains(extension))
                        throw new ArgumentException($"Ảnh mới: Chỉ hỗ trợ định dạng JPG, JPEG, PNG.");

                    if (image.Length > maxImageSize)
                        throw new ArgumentException($"Ảnh mới: Dung lượng vượt quá 5MB (hiện tại: {Math.Round((double)image.Length / 1024 / 1024, 1)}MB).");

                    // Optional: Check ContentType
                    if (!image.ContentType.StartsWith("image/"))
                        throw new ArgumentException($"Ảnh mới: Loại file không phải ảnh (ContentType: {image.ContentType}).");
                }
                // Upload ảnh mới (chỉ nếu valid)
                foreach (var img in post_images)
                {
                    var newName = await _imageHelper.UploadImageWithThumbnailAsync(img, "posts", 600);
                    imagesToKeep.Add(newName);
                }
            }

            // 🔥 XỬ LÝ VIDEO MỚI (tương tự CreatePostAsync)
            string existingVideo = existingPost.post_video ?? "";  // Video hiện tại từ DB
            string? videoName = existingVideo;  // Mặc định: Giữ nguyên video cũ

            if (post_video != null && post_video.Length > 0)
            {
                // 👈 THÊM: Validate video mới theo BR-40 (type, size - tương tự Create)
                var allowedVideoExtensions = new[] { ".mp4", ".mov" };
                const long maxVideoSize = 100 * 1024 * 1024;  // 100MB

                if (post_video.Length == 0)
                    throw new ArgumentException("File video mới không hợp lệ (rỗng).");

                var extension = Path.GetExtension(post_video.FileName).ToLowerInvariant();
                if (!allowedVideoExtensions.Contains(extension))
                    throw new ArgumentException($"Video mới: Chỉ hỗ trợ định dạng MP4, MOV.");

                if (post_video.Length > maxVideoSize)
                    throw new ArgumentException($"Video mới: Dung lượng vượt quá 100MB (hiện tại: {Math.Round((double)post_video.Length / 1024 / 1024, 1)}MB).");

                // Check ContentType (cải thiện từ code cũ)
                if (!post_video.ContentType.StartsWith("video/"))
                    throw new ArgumentException($"Video mới: Loại file không phải video (ContentType: {post_video.ContentType}).");

                // Xóa video cũ nếu tồn tại (sử dụng method mới cho video)
                if (!string.IsNullOrEmpty(existingVideo))
                {
                    await _S3.DeleteVideoFileAsync("posts/videos", existingVideo);  // 👈 XÓA VIDEO CŨ (KHÔNG THUMB/FULL)
                }

                // Upload video mới (chỉ nếu valid)
                videoName = await _imageHelper.UploadVideoAsync(post_video, "posts/videos");
            }
            else
            {
                // Không có video mới → kiểm tra old_video để quyết định giữ/xóa
                bool userWantsToRemoveVideo = string.IsNullOrEmpty(old_video ?? "");
                if (userWantsToRemoveVideo && !string.IsNullOrEmpty(existingVideo))
                {
                    // User xóa video (old_video rỗng) → xóa file cũ và set null
                    await _S3.DeleteVideoFileAsync("posts/videos", existingVideo);  // 👈 XÓA VIDEO
                    videoName = null;
                }
                else if (!string.IsNullOrEmpty(old_video))
                {
                    // Giữ video cũ (old_video có giá trị, khớp existing)
                    videoName = old_video;  // Hoặc giữ existingVideo (nên khớp nhau)
                }
                // Else: Không thay đổi (videoName = existingVideo)
            }

            // 🔥 CẬP NHẬT POST (thêm post_video)
            var updatedPost = new Post
            {
                post_id = post_id,
                cus_id = cus_id,
                post_content = post_content,
                post_image = imagesToKeep.Any() ? JsonSerializer.Serialize(imagesToKeep) : null,  // Giữ nguyên
                post_video = videoName,  // 👈 THÊM: Set video mới hoặc null
                post_created_date = existingPost.post_created_date,
                post_updated_date = DateTime.Now
            };

            await _apiService.PutAsync<object, Post>($"{ApiEndpoints.POST}/{post_id}", updatedPost);
        }

        /// <summary>
        /// Deletes a post and its associated images and video.
        /// </summary>
        public async Task DeletePostAsync(int post_id)
        {
            var post = await _apiService.GetAsync<Post>($"{ApiEndpoints.POST}/id?ID={post_id}");
            if (post == null) return;

            // Xóa ảnh (giữ nguyên)
            if (!string.IsNullOrEmpty(post.post_image))
            {
                var images = JsonSerializer.Deserialize<List<string>>(post.post_image) ?? new List<string>();
                foreach (var fileName in images)
                {
                    await _S3.DeleteFileAsync("posts", fileName);  // Xóa thumb_ + full_ cho ảnh
                }
            }

            // 👈 THÊM: Xóa video nếu có
            if (!string.IsNullOrEmpty(post.post_video))
            {
                await _S3.DeleteVideoFileAsync("posts/videos", post.post_video);  // Xóa video đơn lẻ
            }

            await _apiService.DeleteAsync<List<Post>>($"{ApiEndpoints.POST}?Id={post_id}");
        }

        /// <summary>
        /// Gets the detail of a single post.
        /// </summary>
        public async Task<PostDetailsViewModel> GetPostDetailsAsync(int postId, int cusId)
        {
            var post = await _apiService.GetAsync<Post>($"{ApiEndpoints.POST}/id?ID={postId}");
            if (post == null) return null;

            var customer = await _apiService.GetAsync<Customer>($"{ApiEndpoints.CUSTOMER}/{cusId}");
            var comments = await _apiService.GetAsync<List<Comment>>(ApiEndpoints.COMMENT);
            var commentList = comments.Where(c => c.post_id == postId).ToList();
            var customers = await _apiService.GetAsync<List<Customer>>(ApiEndpoints.CUSTOMER);
            var likes = await _apiService.GetAsync<List<Like>>(ApiEndpoints.LIKE);

            post.customer = customers.FirstOrDefault(c => c.cus_id == post.cus_id);
            post.post_like_count = likes.Count(l => l.post_id == postId);
            post.post_comment_count = commentList.Count;

            return new PostDetailsViewModel
            {
                Post = post,
                Customer = customer,
                Comments = commentList,
                CommentAuthors = customers,
                Likes = likes,
            };
        }
    }
}
