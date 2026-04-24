using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Services;
using Microsoft.AspNetCore.Http;
using Core.Common;
using Core.Models.Client.AuthModel;
using System.Text.Json;
using System.Net.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Core.Constant;
using System.Text;
namespace DineGO_Client.Controllers
{
    /// <summary>
    /// Handles authentication actions such as login, registration, logout, and password recovery.
    /// </summary>
    /// <author>Phuonghh;Khoinv;Sieuhdd;Thangtm</author>
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly ApiService _apiService;
        public AuthController(ILogger<AuthController> logger, ApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;
        }
        [HttpPost]
        public async Task<IActionResult> SendOtp(string email)
        {
            try
            {
                var response = await _apiService.PostAsync<JsonElement, string>(
                    ApiEndpoints.AUTH_SEND_OTP, email);

                bool success = response.GetProperty("success").GetBoolean();
                string message = response.GetProperty("message").GetString();
                int retryAfter = response.GetProperty("retryAfter").GetInt32();

                return Json(new { success, message, retryAfter });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message, retryAfter = 0 });
            }
        }

        /// <summary>
        /// Displays the login page.
        /// </summary>
        /// <author>Phuonghh</author>
        [HttpGet]
        public IActionResult Login()
        {
            return View(ControllerConstants.LOGIN);
        }

        /// <summary>
        /// Displays the registration page.
        /// </summary>
        /// <author>Phuonghh</author>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Authenticates the user.
        /// </summary>
        /// <param name="username">Username input</param>
        /// <param name="password">Password input</param>
        /// <returns>Redirect to Home on success, or return login view with error.</returns>
        /// <author>Phuonghh</author>
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                var loginData = new { Username = username, Password = password };
                var response = await _apiService.PostAsync<LoginResponse, dynamic>(ApiEndpoints.AUTH_LOGIN, loginData);
                HttpContext.Session.SetString(SessionConstants.TOKEN, response.token);
                HttpContext.Session.SetString(SessionConstants.CUSTOMER_NAME, response.cus_name);
                HttpContext.Session.SetInt32(SessionConstants.CUSTOMER_ID, response.cus_id);
                TempData["SuccessMessage"] = NotificationConstants.LOGIN_SUCCESS;
                return RedirectToAction(ControllerConstants.INDEX, ControllerConstants.HOME);
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Error = ex.Message;
                return View(ControllerConstants.LOGIN);
            }
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <returns>Redirects to Login on success, otherwise returns Register view with validation errors.</returns>
        /// <author>Khoinv;Thangtm</author>
        [HttpPost]
        public async Task<IActionResult> Register(string name, string username, string password, string confirmPassword, string email, string phone, bool gender, string otp)
        {

            if (string.IsNullOrWhiteSpace(name))
                ModelState.AddModelError(KeyConstants.NAME, NotificationConstants.NAME_REQUIRED);
            else if (name.Length < 2)
                ModelState.AddModelError(KeyConstants.NAME, NotificationConstants.NAME_TOO_SHORT);
            else if (!Regex.IsMatch(name, @"^[\p{L}\s]+$"))
                ModelState.AddModelError(KeyConstants.NAME, NotificationConstants.NAME_FORMAT_INVALID);
            else if (name.Length > 100)
                ModelState.AddModelError(KeyConstants.NAME, NotificationConstants.NAME_TOO_LONG);

            if (string.IsNullOrWhiteSpace(username))
                ModelState.AddModelError(KeyConstants.USERNAME, NotificationConstants.USERNAME_REQUIRED);
            else if (username.Length < 4)
                ModelState.AddModelError(KeyConstants.USERNAME, NotificationConstants.USERNAME_TOO_SHORT);
            else if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
                ModelState.AddModelError(KeyConstants.USERNAME, NotificationConstants.USERNAME_FORMAT_INVALID);
            else if (username.Length > 50)
                ModelState.AddModelError(KeyConstants.USERNAME, NotificationConstants.USERNAME_TOO_LONG);

            if (string.IsNullOrWhiteSpace(password))
                ModelState.AddModelError(KeyConstants.PASSWORD, NotificationConstants.PASSWORD_INVALID);
            else if (password.Length < 3)
                ModelState.AddModelError(KeyConstants.PASSWORD, NotificationConstants.PASSWORD_TOO_SHORT);
            else if (password.Length > 255)
                ModelState.AddModelError(KeyConstants.PASSWORD, NotificationConstants.PASSWORD_TOO_LONG);

            if (password != confirmPassword)
                ModelState.AddModelError(KeyConstants.CONFIRM_PASSWORD, NotificationConstants.CONFIRM_PASSWORD_NOT_MATCH);

            if (string.IsNullOrWhiteSpace(email) || !new EmailAddressAttribute().IsValid(email))
                ModelState.AddModelError(KeyConstants.EMAIL, NotificationConstants.EMAIL_INVALID);
            else if (email.Length < 6)
                ModelState.AddModelError(KeyConstants.EMAIL, NotificationConstants.EMAIL_TOO_SHORT);
            else if (email.Length > 100)
                ModelState.AddModelError(KeyConstants.EMAIL, NotificationConstants.EMAIL_TOO_LONG);

            if (string.IsNullOrWhiteSpace(phone) || !Regex.IsMatch(phone, @"^0\d{9,10}$"))
                ModelState.AddModelError(KeyConstants.PHONE, NotificationConstants.PHONE_INVALID);
            else if (phone.Length < 10)
                ModelState.AddModelError(KeyConstants.PHONE, NotificationConstants.PHONE_TOO_SHORT);
            else if (phone.Length > 20)
                ModelState.AddModelError(KeyConstants.PHONE, NotificationConstants.PHONE_TOO_LONG);



            if (!ModelState.IsValid) return View();

            var registerData = new
            {
                Username = username,
                Gender = gender,
                Password = password,
                Name = name,
                Email = email,
                Phone = phone,
                Otp = otp
            };

            try
            {
                System.Text.Json.JsonElement response;

                try
                {
                    response = await _apiService.PostAsync<JsonElement, object>(
                        ApiEndpoints.AUTH_REGISTER, registerData
                    );

                    if (response.TryGetProperty(KeyConstants.MESSAGE, out var msgElement))
                    {
                        var msg = msgElement.GetString();

                        if (msg == DTOConstants.USER_REGISTERED_SUCCESSFULLY)
                        {
                            TempData[KeyConstants.SUCCESS_MESSAGE] = NotificationConstants.REGISTER_SUCCESS;
                            return RedirectToAction(ControllerConstants.LOGIN, ControllerConstants.AUTH);
                        }
                        else if (msg == NotificationConstants.USERNAME_ALREADY_EXISTS)
                        {
                            ModelState.AddModelError(KeyConstants.USERNAME, NotificationConstants.USERNAME_EXISTED);
                        }
                        else if (msg == NotificationConstants.EMAIL_ALREADY_EXISTS)
                        {
                            ModelState.AddModelError(KeyConstants.EMAIL, NotificationConstants.EMAIL_EXISTED);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, NotificationConstants.SERVER_INVALID_RESPONSE);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, NotificationConstants.SERVER_INVALID_RESPONSE);
                    }
                }
                catch (System.Text.Json.JsonException jsonEx)
                {
                    var fallbackResponse = jsonEx.Message;

                    if (fallbackResponse.Contains(KeyConstants.EMAIL, StringComparison.OrdinalIgnoreCase))
                        ModelState.AddModelError(KeyConstants.EMAIL, NotificationConstants.EMAIL_EXISTED);
                    else if (fallbackResponse.Contains(KeyConstants.USERNAME, StringComparison.OrdinalIgnoreCase))
                        ModelState.AddModelError(KeyConstants.USERNAME, NotificationConstants.USERNAME_EXISTED);
                    else
                        ModelState.AddModelError(string.Empty, NotificationConstants.UNKNOWN_ERROR);
                }
            }
            catch (HttpRequestException ex)
            {
                var msg = ex.Message.ToLower();
                if (msg.Contains(KeyConstants.USERNAME))
                    ModelState.AddModelError(KeyConstants.USERNAME, NotificationConstants.USERNAME_EXISTED);
                else if (msg.Contains(KeyConstants.EMAIL))
                    ModelState.AddModelError(KeyConstants.EMAIL, NotificationConstants.EMAIL_EXISTED);
                else
                    ModelState.AddModelError(string.Empty, NotificationConstants.SERVER_ERROR);
            }

            return View();
        }


        /// <summary>
        /// Logs out the current user.
        /// </summary>
        /// <returns>Redirects to login page.</returns>
        /// <author>Thangtm</author>
        public IActionResult Logout()
        {
            HttpContext.Session.Remove(SessionConstants.TOKEN);
            HttpContext.Session.Remove(SessionConstants.CUSTOMER_ID);
            HttpContext.Session.Remove(SessionConstants.RESTAURANT_ID);
            HttpContext.Session.Remove(SessionConstants.RESTAURANT_OWNER_ID);
            TempData["SuccessMessage"] = NotificationConstants.LOGOUT_SUCCESS;
            return RedirectToAction(ControllerConstants.LOGIN);
        }

        /// <summary>
        /// Displays the Forgot Password page.
        /// </summary>
        /// <author>Sieuhdd</author>
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Handles the Forgot Password logic by calling API to send new password to email.
        /// </summary>
        /// <param name="email">User's email</param>
        /// <returns>ForgotPassword view with success or error message.</returns>
        /// <author>Sieuhdd</author>
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Message = NotificationConstants.EMAIL_REQUIRED;
                return View();
            }

            // Gửi yêu cầu đến API
            var response = await _apiService.GetAsync<JsonElement>(
                string.Format(ApiEndpoints.AUTH_FORGOT_PASSWORD, email)
            );

            if (response.TryGetProperty(KeyConstants.MESSAGE, out JsonElement messageElement) && messageElement.GetString() == "Email does not exist.")
            {
                ViewBag.Message = NotificationConstants.EMAIL_NOT_EXIST;
                return View();
            }

            ViewBag.Message = NotificationConstants.PASSWORD_SENT;
            return View();
        }

        /// <summary>
        /// DTO for carrying the Google ID token from the client to the server.
        /// </summary>
        public class GoogleIdTokenRequest
        {
            /// <summary>
            /// The opaque credential string returned by Google Sign-In.
            /// </summary>
            public string idToken { get; set; }
        }

        /// <summary>
        /// Endpoint for processing Google Sign-In on the server side.
        /// Receives a Google ID token, exchanges it for our own JWT, and stores
        /// the session cookie for subsequent requests.
        /// </summary>
        /// <param name="model">The deserialized payload containing the Google ID token.</param>
        /// <returns>
        /// A JSON object indicating success or failure. On success, the user’s session
        /// is initialized; on failure, an error message is returned.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> LoginWithGoogleToken([FromBody] GoogleIdTokenRequest model)
        {
            try
            {
                // Forward the Google ID token to our API which validates it and returns our JWT.
                var response = await _apiService.PostAsync<LoginResponse, GoogleIdTokenRequest>(
                    ApiEndpoints.AUTH_GOOGLE_TOKEN,
                    model
                );

                // Store the returned JWT and user details in the ASP.NET session.
                HttpContext.Session.SetString(SessionConstants.TOKEN, response.token);
                HttpContext.Session.SetString(SessionConstants.CUSTOMER_NAME, response.cus_name);
                HttpContext.Session.SetInt32(SessionConstants.CUSTOMER_ID, response.cus_id);
                TempData["SuccessMessage"] = NotificationConstants.LOGIN_SUCCESS;
                // Indicate success to the client script.
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // On any failure, return a JSON object describing the error.
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}