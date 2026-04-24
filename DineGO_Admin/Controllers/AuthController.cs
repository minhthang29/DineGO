 
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Services;
using Core.Constant;
using Microsoft.AspNetCore.Http;
using Core.Common;
using System.Text.Json;
using System.Net.Http;
using Core.Models.AdminModel.AuthModel;

namespace DineGO_Admin.Controllers
{
    /// <summary>
    /// Controller for authentication actions such as login, register, and logout.
    /// </summary>
    /// <author>Phuonghh</author>
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly ApiService _apiService;
        public AuthController(ILogger<AuthController> logger, ApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;
        }

        /// <summary>
        /// Displays the login page.
        /// </summary>
        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }

        /// <summary>
        /// Handles login POST request.
        /// </summary>
        /// <param name="username">Admin username.</param>
        /// <param name="password">Admin password.</param>
        /// <returns>Redirects to Home on success, returns login view on failure.</returns>
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                var loginData = new { Username = username, Password = password };
                var response = await _apiService.PostAsync<LoginResponse, dynamic>("Auth/loginAdmin", loginData);
                if (response != null)
                {
                    HttpContext.Session.SetString("token", response.ad_token);
                    HttpContext.Session.SetInt32("ad_id", response.ad_id);
                    HttpContext.Session.SetString("ad_name", response.ad_name);
                    HttpContext.Session.SetString("ad_image", response.ad_image ?? "1.png");
                }
                return RedirectToAction("Index", "Home");
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Error = ex.Message;
                return View(ControllerConstants.LOGIN);
            }
        }

        /// <summary>
        /// Logs out the current admin and clears session.
        /// </summary>
        /// <returns>Redirects to login page.</returns>
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("token");
            HttpContext.Session.Remove("ad_id");
            HttpContext.Session.Remove("ad_name");
            return RedirectToAction("Login");
        }

    }
}