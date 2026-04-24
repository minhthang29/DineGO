using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Constant;
using Core.Services;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DineGO_Client.Controllers
{
    /// <summary>
    /// Manages blog-related operations such as listing and viewing blog details.
    /// </summary>
    /// <author>Sieuhdd</author>
    public class BlogController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<BlogController> _logger;
        public BlogController(ApiService apiService, ILogger<BlogController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }
        /// <summary>
        /// Retrieves a list of blogs from the API and displays them.
        /// </summary>
        /// <returns>A view displaying the list of blogs.</returns>
        public async Task<IActionResult> Index()
        {
            var response = await _apiService.GetAsync<List<Blog>>(ApiEndpoints.BLOG);
            return View(response);
        }
        /// <summary>
        /// Retrieves blog details by ID from the API.
        /// </summary>
        /// <param name="id">The ID of the blog to retrieve.</param>
        /// <returns>A view displaying the blog detail.</returns>
        public async Task<IActionResult> ViewBlogDetail(int id)
        {
            var response = await _apiService.GetAsync<Blog>($"{ApiEndpoints.BLOG_BY_ID}{id}");
            return View(response); // Returns empty model if failed
        }

    }
}