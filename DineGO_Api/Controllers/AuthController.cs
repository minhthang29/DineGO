using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Core.Services;
using Core.Constant;
using Core.Models;
using Core.Modelss.AuthModel;
using DineGO_Api.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http;
using Core.Common;
using System.IO;
using Microsoft.AspNetCore.Http;
using DineGO_Api.Services;

namespace DineGO_Api.Controllers
{
    /// <summary>
    /// Handles user authentication operations including registration, login, and password reset.
    /// </summary>
    /// <author>Phuonghh, Thangtm, Sieuhdd</author>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly HashService _hashService;
        private readonly TokenService _tokenService;
        private readonly ICustomerRepository _customerReository;
        private readonly IAdminRepository _adminReository;
        private readonly IMailSenderRepository _mailSenderRepository;

        private readonly ImageHelper _imageHelper;
        private readonly S3BucketAWS _S3;

        private readonly OtpService _otpService;

        /// <summary>
        /// Constructor that injects required services and repositories.
        /// </summary>
        public AuthController(ApplicationDbContext context, TokenService tokenService, HashService hashService, ICustomerRepository customerRepository, IMailSenderRepository mailSenderRepository, IAdminRepository adminReository, S3BucketAWS S3, ImageHelper imageHelper, OtpService otpService)
        {
            _context = context;
            _tokenService = tokenService;
            _hashService = hashService;
            _customerReository = customerRepository;
            _mailSenderRepository = mailSenderRepository;
            _adminReository = adminReository;
            _S3 = S3;
            _imageHelper = imageHelper;
            _otpService = otpService;
        }

        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] string email)
        {
            var (ok, msg, wait) = await _otpService.SendRegistrationOtpAsync(email);
            if (!ok)
                return StatusCode(wait.HasValue ? 429 : 400,
                    new { success = false, message = msg, retryAfter = wait ?? 0 });

            return Ok(new { success = true, message = msg, retryAfter = wait ?? 0 });
        }

        [HttpPost("check-otp")]
        public IActionResult CheckOtp([FromQuery] string email, [FromQuery] string otp)
        {
            var st = _otpService.CheckRegistrationOtp(email, otp);

            if (st == OtpStatus.Ok) return Ok(new { valid = true, message = "OTP hợp lệ" });
            if (st == OtpStatus.TooManyAttempts) return BadRequest(new { valid = false, message = "Quá số lần thử, vui lòng gửi lại OTP." });
            return BadRequest(new { valid = false, message = "OTP sai hoặc đã hết hạn." });
        }

        /// <summary>
        /// Registers a new customer account.
        /// If an account already exists with the same email via Google login,
        /// it upgrades that account to support manual login (adds password).
        /// </summary>
        /// <param name="registerRequest">The registration request payload</param>
        /// <returns>Status 200 on success, 400 on duplicate username</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            // OTP validation
            var otp = registerRequest.Otp;
            var st = _otpService.CheckRegistrationOtp(registerRequest.Email, otp);
            if (st == OtpStatus.TooManyAttempts)
                return BadRequest("Bạn đã vượt quá số lần thử, vui lòng gửi lại OTP.");
            if (st != OtpStatus.Ok)
                return BadRequest("OTP sai hoặc đã hết hạn.");

            var existing = _context.Customers.FirstOrDefault(u => u.cus_username == registerRequest.Username);

            if (existing != null)
            {
                if (existing.login_provider == "Google")
                {
                    // Merge: allow user to add password to an existing Google-based account
                    existing.cus_password = _hashService.HashPassword(registerRequest.Password);
                    existing.cus_name = string.IsNullOrEmpty(existing.cus_name)
                        ? registerRequest.Name
                        : existing.cus_name;
                    existing.cus_phone = string.IsNullOrEmpty(existing.cus_phone)
                        ? registerRequest.Phone
                        : existing.cus_phone;

                    // Remove Google login markers so manual login is now allowed
                    existing.login_provider = null;
                    existing.google_id = null;

                    _context.Customers.Update(existing);
                    _context.SaveChanges();

                    return Ok(new
                    {
                        Message = NotificationConstants.GOOGLE_ACCOUNT_LINKED
                    });
                }

                // If the account is already a normal user, block registration
                return BadRequest(NotificationConstants.USERNAME_ALREADY_EXISTS);
            }

            // New user, create a fresh account
            var cus = new Customer
            {
                cus_name = registerRequest.Name,
                cus_gender = registerRequest.Gender,
                cus_username = registerRequest.Username,
                cus_password = _hashService.HashPassword(registerRequest.Password),
                cus_email = registerRequest.Email,
                cus_phone = registerRequest.Phone,
                cus_image = "default.jpeg", // Default image, can be updated later
                cus_is_use = true,
                login_provider = null,
                google_id = null
            };

            _context.Customers.Add(cus);
            _context.SaveChanges();

            return Ok(new { Message = DTOConstants.USER_REGISTERED_SUCCESSFULLY });
        }


        /// <summary>
        /// Logs in a user and returns a JWT token if credentials are valid.
        /// </summary>
        /// <param name="loginRequest">The login credentials.</param>
        /// <returns>JWT token and basic user info or 400 error if invalid.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var user = _context.Customers.SingleOrDefault(u => u.cus_username == loginRequest.Username && !u.cus_is_deleted);

            if (user == null || !_hashService.VerifyPassword(loginRequest.Password, user.cus_password))
                return BadRequest(NotificationConstants.INVALID_USERNAME_OR_PASSWORD);
            if (!user.cus_is_use)
                return BadRequest("Tài khoản bị khoá hoặc không được kích hoạt");
            var token = _tokenService.GenerateToken(loginRequest.Username, "User");

            return Ok(new { token = token, cus_id = user.cus_id, cus_name = user.cus_name });
        }

        [HttpPost("loginAdmin")]
        public async Task<IActionResult> LoginAdmin([FromBody] LoginRequest loginRequest)
        {
            var admin = _context.Admins.SingleOrDefault(u => u.ad_username == loginRequest.Username);

            if (admin == null || !_hashService.VerifyPassword(loginRequest.Password, admin.ad_password))
                return BadRequest(NotificationConstants.INVALID_USERNAME_OR_PASSWORD);

            var ad_token = _tokenService.GenerateToken(loginRequest.Username, "Admin");

            return Ok(new { ad_token = ad_token, ad_id = admin.ad_id, ad_name = admin.ad_name, ad_image = admin.ad_image });
        }

        /// <summary>
        /// Sends a new password to the user's email if it exists in the system.
        /// </summary>
        /// <param name="email">The email address to send the new password to.</param>
        /// <returns>Status 200 if sent, 404 if email does not exist.</returns>
        [HttpGet("forgetpassword")]
        public IActionResult ForgetPassword([FromQuery] string email)
        {
            var customer = _customerReository.IsMailExist(email);

            if (customer == null)
            {
                return NotFound(new { message = NotificationConstants.EMAIL_NOT_EXIST });
            }

            string newPassword = _hashService.GenerateRandomPassword();
            string hashedPassword = _hashService.HashPassword(newPassword);

            _customerReository.ChangPassword(email, hashedPassword);
            _mailSenderRepository.SendMail(email, "Reset Mật Khẩu", () => $"Mật khẩu mới của bạn là: {newPassword}");

            return Ok(new { message = NotificationConstants.PASSWORD_SENT });
        }

        /// <summary>
        /// Initiates the Google login process by redirecting the user to the Google authentication page.
        /// </summary>
        /// <returns>Redirects the user to Google's OAuth login page.</returns>
        [HttpGet("login-google")]
        public IActionResult LoginWithGoogle()
        {
            var redirectUrl = Url.Action("GoogleResponse", ControllerConstants.AUTH);
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Callback action after Google login is completed. 
        /// Retrieves user information such as email and name from Google.
        /// </summary>
        /// <returns>Returns the user's email and name if authentication succeeded.</returns>
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;

            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            return Ok(new { email, name });
        }

        /// <summary>
        /// Handles Google Sign-In via ID token (client-side flow).
        /// If a user with the same email already exists, it links the Google account
        /// to the existing user (account merging). Otherwise, creates a new account.
        /// </summary>
        /// <param name="request">Request containing the Google ID token from client-side login</param>
        /// <returns>
        /// Returns a JWT token and user info if login is successful, or an error if invalid.
        /// </returns>
        [HttpPost("google-token")]
        public async Task<IActionResult> LoginWithGoogleToken([FromBody] GoogleLoginTokenRequest request)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(request.idToken);
            // Extract claims from Google ID token
            var email = token.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var name = token.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            var picture = token.Claims.FirstOrDefault(c => c.Type == "picture")?.Value;
            var googleId = token.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            string fileName = "default.jpeg"; // fallback mặc định
            // Try to find an existing user with this email
            var user = _context.Customers.FirstOrDefault(c => c.cus_email == email);

            if (!string.IsNullOrEmpty(picture) && user == null)
            {
                try
                {
                    var httpClient = new HttpClient();
                    var imageBytes = await httpClient.GetByteArrayAsync(picture);
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        var stream = new MemoryStream(imageBytes);
                        IFormFile file = new FormFile(stream, 0, imageBytes.Length, "picture", "avatar.jpg");

                        // Gán ContentType thủ công nếu null
                        var contentType = "image/jpeg"; // hoặc đoán theo đuôi file nếu cần
                        if (file is FormFile formFile)
                        {
                            formFile.Headers = new HeaderDictionary();
                            // Không thể set ContentType trực tiếp, nên khi truyền sang S3BucketAWS, hãy truyền contentType này riêng
                        }

                        fileName = await _imageHelper.UploadImageWithThumbnailAsync(file, "customers", thumbWidth: 600);
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"Error downloading image: {ex.Message}");
                }
            }
            if (string.IsNullOrEmpty(email))
                return BadRequest(NotificationConstants.TOKEN_INVALID_EMAIL);

            

            if (user == null)
            {
                // No existing user → create a new one using Google profile
                user = new Customer
                {
                    cus_email = email,
                    cus_name = name,
                    cus_image = fileName,
                    cus_username = email,
                    cus_password = "", // No password required for Google accounts
                    cus_phone = "",
                    login_provider = "Google",
                    google_id = googleId,
                    cus_is_use = true // Default to active
                };
                _context.Customers.Add(user);
            }
            else
            {
                if (!user.cus_is_use)
                    return BadRequest("Tài khoản bị khoá hoặc không được kích hoạt");
                if (user.cus_is_deleted)
                    return BadRequest("Tài khoản đã bị xoá");
                // Existing user found → merge Google info if not already linked
                if (string.IsNullOrEmpty(user.login_provider))
                {
                    user.login_provider = "Google";
                    user.google_id = googleId;
                }

                // Optionally update name if it was not set
                if (string.IsNullOrEmpty(user.cus_name) && !string.IsNullOrEmpty(name))
                {
                    user.cus_name = name;
                }

                _context.Customers.Update(user);
            }

            _context.SaveChanges();

            // Issue JWT token for authenticated access
            var jwt = _tokenService.GenerateToken(user.cus_username, "User");

            return Ok(new { token = jwt, cus_id = user.cus_id, cus_name = user.cus_name });
        }
    }
}
