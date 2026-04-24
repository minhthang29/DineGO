using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Models;
using Core.Models.Client;
using Core.Services;
using Core.Constant;
using Microsoft.AspNetCore.Http;
using Core.Services;
using Core.Models.Client.Custom;
namespace DineGO_Client.Controllers
{
    /// <summary>
    /// Handles the home page and error page actions.
    /// </summary>
    /// <author>Phuonghh,KhoiNV</author>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RestaurantService _restaurantService;
        private readonly BlogService _blogService;
        private readonly AdService _adService;


        public HomeController(ILogger<HomeController> logger, RestaurantService restaurantService, BlogService blogService, AdService adService)
        {
            _logger = logger;
            _restaurantService = restaurantService;
            _blogService = blogService;
            _adService = adService;
        }
        /// <summary>
        /// Displays the home page of the application.
        /// </summary>
        /// <returns>Returns the view for the home page.</returns>
        /// <author>KhoiNV</author>
        public async Task<IActionResult> Index()
        {
            var restaurants = await _restaurantService.GetAllRestaurantsAsync();
            var blogs = await _blogService.GetAllBlogsAsync();

            var viewModel = new CustomHomeViewModel
            {
                Restaurants = restaurants,
                Blogs = blogs
            };

            // Lấy quảng cáo đang chạy
            var ads = await _adService.GetAdsByStatusAsync(true);

            // Banner ads
            var bannerAds = ads.Where(a => a.slot_type == 1).ToList();
            ViewBag.BannerAds = bannerAds;

            // Popup ads
            var popupAds = ads.Where(a => a.slot_type == 2).ToList();
            if (popupAds.Any())
            {
                // chọn ngẫu nhiên 1 popup
                var random = new Random();
                var selected = popupAds[random.Next(popupAds.Count)];
                ViewBag.PopupAd = selected;
            }

            return View(viewModel);
        }
        /// <summary>
        /// Displays the privacy page of the application.
        /// </summary>
        /// <returns>Returns the view for the privacy page.</returns>
        public IActionResult Privacy()
        {
            return View();
        }
        /// <summary>
        /// Displays the error page with the error message.
        /// </summary>
        /// <returns>Returns the error view with the error details.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorMessage = HttpContext.Session.GetString(KeyConstants.ERROR_MESSAGE);
            var model = new ErrorViewModel
            {
                ErrorMessage = errorMessage,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View(model);
        }

    }
}
