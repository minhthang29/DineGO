using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Constant;
using Core.Services;
using Core.Models;
using Core.Common;

namespace DineGO_Admin.Controllers
{
    [Route("[controller]")]
    public class PostController : Controller
    {
        private readonly ApiService _apiService;

        public PostController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(string status = "", string search = "", int page = 1)
        {
            var posts = await _apiService.GetAsync<List<Post>>(ApiEndpoints.POST);
            
            // Load customer data for each post
            foreach (var post in posts)
            {
                if (post.cus_id.HasValue)
                {
                    post.customer = await _apiService.GetAsync<Customer>($"{ApiEndpoints.CUSTOMER}/{post.cus_id}");
                }
            }

            // Filter by status
            if (!string.IsNullOrEmpty(status))
            {
                switch (status.ToLower())
                {
                    case "approved":
                        posts = posts.Where(p => p.post_is_approve).ToList();
                        break;
                    case "pending":
                        posts = posts.Where(p => !p.post_is_approve).ToList();
                        break;
                }
            }

            // Filter by search
            if (!string.IsNullOrEmpty(search))
            {
                posts = posts.Where(p => 
                    p.post_title?.Contains(search, StringComparison.OrdinalIgnoreCase) == true ||
                    p.post_content?.Contains(search, StringComparison.OrdinalIgnoreCase) == true ||
                    p.customer?.cus_name?.Contains(search, StringComparison.OrdinalIgnoreCase) == true
                ).ToList();
            }

            // Sort by created date (newest first)
            posts = posts.OrderByDescending(p => p.post_created_date).ToList();

            ViewBag.CurrentStatus = status;
            ViewBag.CurrentSearch = search;
            ViewBag.TotalPosts = posts.Count;
            ViewBag.ApprovedCount = posts.Count(p => p.post_is_approve);
            ViewBag.PendingCount = posts.Count(p => !p.post_is_approve);

            return View(posts);
        }

        [HttpGet("PendingApproval")]
        public async Task<IActionResult> PendingApproval()
        {
            var posts = await _apiService.GetAsync<List<Post>>(ApiEndpoints.POST);
            
            // Filter only pending posts
            posts = posts.Where(p => !p.post_is_approve).ToList();
            
            // Load customer data
            foreach (var post in posts)
            {
                if (post.cus_id.HasValue)
                {
                    post.customer = await _apiService.GetAsync<Customer>($"{ApiEndpoints.CUSTOMER}/{post.cus_id}");
                }
            }

            posts = posts.OrderByDescending(p => p.post_created_date).ToList();
            
            return View(posts);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var post = await _apiService.GetAsync<Post>($"{ApiEndpoints.POST}/id?ID={id}");
            
            if (post == null)
            {
                return NotFound();
            }

            // Load customer data
            if (post.cus_id.HasValue)
            {
                post.customer = await _apiService.GetAsync<Customer>($"{ApiEndpoints.CUSTOMER}/{post.cus_id}");
            }

            return View(post);
        }

        [HttpPost("Approve/{id}")]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                // Gọi trực tiếp API approve endpoint
                var result = await _apiService.PutAsync<object, object>($"{ApiEndpoints.POST}/{id}/approve", null);

                TempData["SuccessMessage"] = "Bài viết đã được duyệt thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi duyệt bài viết: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost("Reject/{id}")]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                // Gọi trực tiếp API reject endpoint
                var result = await _apiService.PutAsync<object, object>($"{ApiEndpoints.POST}/{id}/reject", null);

                TempData["SuccessMessage"] = "Bài viết đã được từ chối!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi từ chối bài viết: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _apiService.DeleteAsync<object>($"{ApiEndpoints.POST}?Id={id}");
                TempData["SuccessMessage"] = "Bài viết đã được xóa thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa bài viết: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}