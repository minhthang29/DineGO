using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Services;
using Core.Models;
using Core.Constant;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Common;
using Microsoft.AspNetCore.Http; 

namespace DineGO_Admin.Controllers
{
    public class BlogController : Controller
    {
        private readonly ILogger<BlogController> _logger;
        private readonly BlogService _blogService;
        private readonly ImageHelper _imageHelper;
        private readonly S3BucketAWS _S3;

        public BlogController(ILogger<BlogController> logger, BlogService blogService , ImageHelper imageHelper, S3BucketAWS S3)
        {
            _logger = logger;
            _blogService = blogService;
            _imageHelper = imageHelper;
            _S3 = S3;
        }

        public async Task<IActionResult> Index()
        {
            var blogs = await _blogService.GetAllBlogsAsync();
            return View(blogs);
        }

        [HttpGet]
        public IActionResult AddBlog()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBlog(Blog blog, IFormFile blog_image_file)
        {
            if (ModelState.IsValid)
            {
                if (blog_image_file != null && blog_image_file.Length > 0)
                {
                    var fileName = await _imageHelper.UploadImageWithThumbnailAsync(blog_image_file, "blogs", thumbWidth: 600);
                    blog.blog_image = fileName;
                }
                blog.blog_date = DateTime.Now;
                await _blogService.AddAsync(blog);
                TempData[KeyConstants.SUCCESS_MESSAGE] = NotificationConstants.BLOG_CREATE_SUCCESS;
                return RedirectToAction("Index");
            }
            TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.BLOG_CREATE_FAILED;
            return View(blog);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateBlog(int id)
        {
            var blog = await _blogService.GetByIdAsync(id);
            if (blog == null)
            {
                TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.BLOG_NOT_FOUND;
                return RedirectToAction("Index");
            }
            return View(blog);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBlog(Blog blog, IFormFile blog_image_file)
        {
            if (ModelState.IsValid)
            {
                if (blog_image_file != null && blog_image_file.Length > 0)
                {
                    var fileName = await _imageHelper.UploadImageWithThumbnailAsync(blog_image_file, "blogs", thumbWidth: 600);
                    blog.blog_image = fileName;
                }
                else
                {
                    // Lấy lại ảnh cũ nếu không upload mới
                    var oldBlog = await _blogService.GetByIdAsync(blog.blog_id);
                    if (oldBlog != null)
                        blog.blog_image = oldBlog.blog_image;
                }
                blog.blog_date = DateTime.Now;
                await _blogService.UpdateAsync(blog);
                TempData[KeyConstants.SUCCESS_MESSAGE] = NotificationConstants.BLOG_UPDATE_SUCCESS;
                return RedirectToAction("Index");
            }
            TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.BLOG_UPDATE_FAILED;
            return View(blog);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var blog = await _blogService.GetByIdAsync(id);
            if (blog == null)
            {
                TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.BLOG_NOT_FOUND;
                return RedirectToAction("Index");
            }
            return View(blog);
        }

        [HttpPost, ActionName("DeleteBlog")]
        public async Task<IActionResult> DeleteBlogConfirmed(int id)
        {
            try
            {
                await _blogService.DeleteAsync(id);
                TempData[KeyConstants.SUCCESS_MESSAGE] = NotificationConstants.BLOG_DELETE_SUCCESS;
            }
            catch
            {
                TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.BLOG_DELETE_FAILED;
            }
            return RedirectToAction("Index");
        }
    }
}