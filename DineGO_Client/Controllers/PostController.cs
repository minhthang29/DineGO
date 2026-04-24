using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Core.Constant;
using Core.Services;
using Core.Common;
using Core.Models.Client.Custom;
using Core.Models;
using System.Text.Json;

namespace DineGO_Client.Controllers
{
    /// <summary>
    /// Controller for managing post of customer
    /// </summary>
    /// <author>thangtm</author>
    public class PostController : Controller
    {
        private readonly PostService _postService;
        private readonly ApiService _apiService;

        public PostController(PostService postService, ApiService apiService)
        {
            _postService = postService;
            _apiService = apiService;
        }

        /// <summary>
        /// Returns the list of posts and related data for display.
        /// </summary>
        /// <param name="showMine">If true, only shows the current user's posts.</param>
        /// <returns>View containing the post data.</returns>
        public async Task<IActionResult> Index(bool showMine = false)
        {
            int? cusId = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            if (cusId == null) throw new UnauthorizedAccessException();
            ViewBag.CusId = cusId;
            var viewModel = await _postService.GetAllAsync(showMine, cusId);
            return View(viewModel);
        }

        /// <summary>
        /// Creates a new post with content and image.
        /// </summary>
        /// <param name="post_content">Content of the post.</param>
        /// <param name="post_image">Image file uploaded for the post.</param>
        /// <returns>Redirects to post list after creation.</returns>
        [HttpPost]
        public async Task<IActionResult> Create(
    string post_content,
    List<IFormFile>? post_images,
    IFormFile? post_video) // 👈 thêm video
        {
            var cusId = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            if (cusId == null) throw new UnauthorizedAccessException();

            if (string.IsNullOrWhiteSpace(post_content) ||
                (post_images == null || post_images.Count == 0) &&
                (post_video == null))
            {
                TempData[KeyConstants.ERROR_MESSAGE] = "Bài viết phải có nội dung, ảnh hoặc video.";
                return RedirectToAction("Index");
            }

            try
            {
                await _postService.CreatePostAsync(cusId.Value, post_content, post_images, post_video);
                TempData["SuccessMessage"] = NotificationConstants.CREATE_POST_SUCCESS;
                return RedirectToAction("Index", new { showMine = true });
            }
            catch (ArgumentException ex)
            {
                TempData[KeyConstants.ERROR_MESSAGE] = ex.Message;
                return RedirectToAction("Index");
            }
        }


        /// <summary>
        /// Edits an existing post with optional updated image.
        /// </summary>
        /// <param name="post_id">ID of the post to edit.</param>
        /// <param name="post_content">Updated content of the post.</param>
        /// <param name="post_image">New image file (optional).</param>
        /// <returns>Redirects to post list after edit.</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(
    int post_id,
    string post_content,
    string old_images,
    List<IFormFile>? post_images,
    IFormFile? post_video,  // Đảm bảo name="post_video" ở HTML
    string? old_video)
        {
            var cusId = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            if (cusId == null)
            {
                TempData[KeyConstants.ERROR_MESSAGE] = "Vui lòng đăng nhập lại.";
                return RedirectToAction("Index");
            }
            
            if (string.IsNullOrWhiteSpace(post_content) ||
                (post_images == null || post_images.Count == 0) &&
                (post_video == null))
            {
                TempData[KeyConstants.ERROR_MESSAGE] = "Bài viết phải có nội dung, ảnh hoặc video.";
                return RedirectToAction("Index");
            }

            try
            {
                await _postService.EditPostAsync(
                    cusId.Value, post_id, post_content, old_images,
                    post_images ?? new List<IFormFile>(), post_video, old_video);
                TempData["SuccessMessage"] = NotificationConstants.EDIT_POST_SUCCESS;
                return RedirectToAction("Index", new { showMine = true });
            }
            catch (ArgumentException ex)
            {
                TempData[KeyConstants.ERROR_MESSAGE] = ex.Message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)  // Lỗi hệ thống (S3/API)
            {
                TempData[KeyConstants.ERROR_MESSAGE] = $"Lỗi hệ thống khi cập nhật bài viết (video: {post_video?.FileName ?? "không có"}). Vui lòng thử lại hoặc liên hệ admin.";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Deletes a post by its ID.
        /// </summary>
        /// <param name="id">ID of the post to delete.</param>
        /// <returns>Redirects to post list after deletion.</returns>
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _postService.DeletePostAsync(id);
            TempData["SuccessMessage"] = NotificationConstants.DELETE_POST_SUCCESS;
            return Json(new { redirectUrl = Url.Action("Index", "Post") });
        }


        /// <summary>
        /// Retrieves detailed view of a single post.
        /// </summary>
        /// <param name="postId">ID of the post.</param>
        /// <returns>Partial view containing post detail.</returns>
        public async Task<IActionResult> Details(int postId)
        {
            int? cusId = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            var detail = await _postService.GetPostDetailsAsync(postId, cusId.Value);
            if (detail == null) return NotFound();
            return PartialView("_PostDetail", detail);
        }

        /// <summary>
        /// Adds or updates a reaction (like/emotion) to a post.
        /// </summary>
        /// <param name="request">Like object containing post ID, user ID, and emotion type.</param>
        /// <returns>JSON result with updated like count and reaction state.</returns>
        [HttpPost]
        public async Task<IActionResult> ReactToPost([FromBody] Like request)
        {
            int? cusId = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            if (cusId == null)
                return Unauthorized();

            request.cus_id = cusId.Value;

            var result = await _apiService.PostAsync<LikeResponse, Like>(ApiEndpoints.LIKECOUNT, request);

            return Json(new
            {
                reacted = request.like_emotion_type > 0,
                newLikeCount = (int)(result?.newLikeCount ?? 0)
            });
        }

        /// <summary>
        /// Retrieves all reactions for a specific post.
        /// </summary>
        /// <param name="postId">ID of the post.</param>
        /// <returns>Partial view showing all reactions on the post.</returns>
        [HttpGet]
        public async Task<IActionResult> GetReactions(int postId)
        {
            var result = await _apiService.GetAsync<List<ReactionViewModel>>($"{ApiEndpoints.GET_REACTIONS}/{postId}");
            return PartialView("_ReactionList", result);
        }

        /// <summary>
        /// Adds a new comment to a post (AJAX).
        /// </summary>
        /// <param name="comment">Comment object with post ID and content.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddCommentAjax([FromBody] Comment comment)
        {
            if (string.IsNullOrWhiteSpace(comment.comment_content))
                return BadRequest("Nội dung không hợp lệ");
            comment.comment_created_date = DateTime.Now;
            await _apiService.PostAsync<object, Comment>(ApiEndpoints.COMMENT, comment);
            return Ok(new { message = NotificationConstants.CREATE_COMMENT_SUCCESS });
        }


        /// <summary>
        /// Updates the content of an existing comment.
        /// </summary>
        /// <param name="comment">Comment object with updated content.</param>
        /// <returns>Success message and updated timestamp.</returns>
        [HttpPost]
        public async Task<IActionResult> EditComment([FromBody] Comment comment)
        {
            comment.comment_updated_date = DateTime.Now;
            comment.cus_id = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID) ?? 0;
            await _apiService.PutAsync<object, Comment>(ApiEndpoints.COMMENT, comment);
            return Ok(new
            {
                updatedAt = comment.comment_updated_date.ToString("o"),
                message = NotificationConstants.EDIT_COMMENT_SUCCESS
            });
        }

        /// <summary>
        /// Deletes a comment by its ID.
        /// </summary>
        /// <param name="id">ID of the comment to delete.</param>
        /// <returns>Redirects to post list after deletion.</returns>
        [HttpPost]
        public async Task<IActionResult> DeleteComment(int id)
        {
            await _apiService.DeleteAsync<object>($"{ApiEndpoints.COMMENT}?Id={id}");
            return Ok(new { message = NotificationConstants.DELETE_COMMENT_SUCCESS });
        }

    }

    public class LikeResponse
    {
        public bool reacted { get; set; }
        public int newLikeCount { get; set; }
    }
}
