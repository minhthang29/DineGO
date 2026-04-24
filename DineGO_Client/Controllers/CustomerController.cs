using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Services;
using Core.Constant;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Core.Models.Client.Custom;
using Core.Models.Client;
using System.Text.Json;
using Core.Common;
using System.Text.RegularExpressions;

namespace DineGO_Client.Controllers
{
    /// <summary>
    /// Handles customer-related actions such as profile, orders, and password changes.
    /// </summary>
    /// <author>Khoinv;Sieuhdd;Thangtm</author>
    public class CustomerController : Controller
    {
        private readonly ILogger<Customer> _logger;
        private readonly ApiService _apiService;
        private readonly S3BucketAWS _s3Bucket;
        private readonly CustomerService _customerService;
        private readonly ImageHelper _imageHelper;
        private readonly CustomerPointService _pointService;

        public CustomerController(ILogger<Customer> logger, ApiService apiService, S3BucketAWS s3Bucket, CustomerService customerService, ImageHelper imageHelper, CustomerPointService customerPointService)
        {
            _logger = logger;
            _apiService = apiService;
            _s3Bucket = s3Bucket;
            _customerService = customerService;
            _imageHelper = imageHelper;
            _pointService = customerPointService;
        }
        /// <summary>
        /// Returns the default view for customer section.
        /// </summary>
        /// <author>Thangtm</author>
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Displays the customer profile with related data.
        /// </summary>
        /// <author>Thangtm</author>
        public async Task<IActionResult> Profile()
        {
            var cus_id = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            if (cus_id == null)
            {
                throw new UnauthorizedAccessException();
            }
            var viewModel = await _customerService.GetProfileViewModel(cus_id.Value);
            return View(viewModel);
        }

        /// <summary>
        /// Updates the customer profile information.
        /// </summary>
        /// <author>Thangtm</author>
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(CustomerUpdateProfileViewModel customer, IFormFile imageFile)
        {
            var cus_id = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            if (string.IsNullOrWhiteSpace(customer.cus_name))
                ModelState.AddModelError(KeyConstants.CUS_NAME, NotificationConstants.NAME_REQUIRED);
            else if (!Regex.IsMatch(customer.cus_name, @"^[\p{L}\s]+$"))
                ModelState.AddModelError(KeyConstants.CUS_NAME, NotificationConstants.NAME_FORMAT_INVALID);
            if (string.IsNullOrWhiteSpace(customer.cus_phone) || !Regex.IsMatch(customer.cus_phone, @"^0\d{9,10}$"))
                ModelState.AddModelError(KeyConstants.CUS_PHONE, NotificationConstants.PHONE_INVALID);
            if (!ModelState.IsValid)
            {
                // Lấy lại dữ liệu cho CustomProfileViewModel
                var viewModel = await _customerService.GetProfileViewModel(cus_id.Value);
                return View("Profile", viewModel);
            }

            if (cus_id == null) throw new UnauthorizedAccessException();

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = await _imageHelper.UploadImageWithThumbnailAsync(imageFile, "customers", thumbWidth: 200);
                customer.cus_image = fileName;
            }

            var response = await _apiService.PutAsync<object, dynamic>(
                $"{ApiEndpoints.CUSTOMER}/{cus_id}", customer);

            if (response != null)
            {
                TempData["SuccessMessage"] = NotificationConstants.UPDATE_SUCCESS;
            }
            else
            {
                TempData["ErrorMessage"] = NotificationConstants.UPDATE_FAILED;
            }

            return RedirectToAction(ControllerConstants.PROFILE, ControllerConstants.CUSTOMER);
        }

        /// <summary>
        /// Displays the customer's order history.
        /// </summary>
        /// <author>Sieuhdd</author>
        public async Task<IActionResult> OrderHistory()
        {
            int customerId = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID) ?? 0;

            var reservations = await _apiService.GetAsync<List<Reservation>>(
                $"{ApiEndpoints.RESERVATION_BY_CUSID}{customerId}"
            );

            var viewModel = await GetProfileViewModel(customerId, (ControllerConstants.RESERVATION, reservations));

            return View(viewModel);
        }

        /// <summary>
        /// Displays the customer's payment history.
        /// </summary>
        /// <author>Sieuhdd</author>
        public async Task<IActionResult> PaymentHistory()
        {
            int customerId = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID) ?? 0;
            var payments = await _apiService.GetAsync<List<Payment>>($"{ApiEndpoints.PAYMENT_BY_CUSID}{customerId}");

            var viewModel = await GetProfileViewModel(customerId, ("Payments", payments));
            return View(viewModel);
        }

        /// <summary>
        /// Builds the profile view model with additional data.
        /// </summary>
        /// <author>Sieuhdd</author>
        private async Task<CustomProfileViewModel> GetProfileViewModel(int customerId, params (string key, object value)[] extraData)
        {
            var customer = await _apiService.GetAsync<Customer>($"{ApiEndpoints.CUSTOMER}/{customerId}");
            var restaurantOwners = await _apiService.GetAsync<List<RestaurantOwner>>(
                string.Format(ApiEndpoints.RESTAURANT_OWNER_BY_CUS_ID, customerId)
            );

            var restaurants = await _apiService.GetAsync<List<Restaurant>>(ApiEndpoints.RESTAURANT);
            var reservations = await _apiService.GetAsync<List<Reservation>>($"{ApiEndpoints.RESERVATION_BY_CUSID}{customerId}");

            var viewModel = new CustomProfileViewModel
            {
                Customer = customer,
                RestaurantOwners = restaurantOwners,
                Restaurant = restaurants,
                Reservation = reservations
            };

            foreach (var (key, value) in extraData)
            {
                viewModel.Data[key] = value;
            }

            return viewModel;
        }

        /// <summary>
        /// Shows the change password page.
        /// </summary>
        /// <author>Khoinv</author>
        public async Task<IActionResult> ChangePassword()
        {
            int? cus_id = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            if (cus_id == null)
            {
                TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.YOU_NOT_LOGIN;
                return RedirectToAction(ControllerConstants.LOGIN, ControllerConstants.AUTH);
            }

            var customer = await _apiService.GetAsync<Customer>($"{ApiEndpoints.CUSTOMER}/{cus_id}");
            if (customer == null)
            {
                TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.CUSTOMER_NOT_FOUND;
                return RedirectToAction(ControllerConstants.CHANGE_PASSWORD);
            }

            var changePasswordModel = new ChangePasswordViewModel
            {
                Customer = customer,
                HasPassword = !string.IsNullOrWhiteSpace(customer.cus_password)
            };
            var viewModel = new CustomProfileViewModel
            {
                Customer = customer,
                RestaurantOwners = new List<RestaurantOwner>(),
                ChangePasswordModel = changePasswordModel
            };

            return View(viewModel);
        }

        /// <summary>
        /// Handles password change logic for the customer.
        /// </summary>
        /// <author>Khoinv</author>
        [HttpPost]
        public async Task<IActionResult> ChangePassword(CustomProfileViewModel viewModel)
        {
            var cus_id = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            if (cus_id == null)
            {
                TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.YOU_NOT_LOGIN;
                return RedirectToAction(ControllerConstants.LOGIN, ControllerConstants.AUTH);
            }

            var customer = await _apiService.GetAsync<Customer>($"{ApiEndpoints.CUSTOMER}/{cus_id}");

            if (customer == null)
            {
                TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.CUSTOMER_NOT_FOUND;
                return RedirectToAction(ControllerConstants.CHANGE_PASSWORD);
            }

            // Null check for ChangePasswordModel
            if (viewModel.ChangePasswordModel == null)
            {
                viewModel.ChangePasswordModel = new ChangePasswordViewModel();
            }

            var model = viewModel.ChangePasswordModel;

            // Khai báo HashService một lần duy nhất ở đầu
            var hashService = new HashService();

            // Check if customer has existing password
            bool hasExistingPassword = !string.IsNullOrWhiteSpace(customer.cus_password);

            // Validate based on whether customer has password or not
            if (hasExistingPassword)
            {
                // Customer has password - require current password validation
                if (string.IsNullOrWhiteSpace(model.CurrentPassword))
                {
                    ModelState.AddModelError("ChangePasswordModel.CurrentPassword", "Mật khẩu hiện tại là bắt buộc");
                }
                else
                {
                    // Sử dụng hashService đã khai báo ở trên
                    if (!hashService.VerifyPassword(model.CurrentPassword, customer.cus_password))
                    {
                        ModelState.AddModelError("ChangePasswordModel.CurrentPassword", "Mật khẩu hiện tại không đúng");
                    }
                }
            }

            // Validate new password
            if (string.IsNullOrWhiteSpace(model.NewPassword))
            {
                ModelState.AddModelError("ChangePasswordModel.NewPassword", "Mật khẩu mới là bắt buộc");
            }
            else if (model.NewPassword.Length < 6)
            {
                ModelState.AddModelError("ChangePasswordModel.NewPassword", "Mật khẩu phải có ít nhất 6 ký tự");
            }

            // Validate confirm password
            if (string.IsNullOrWhiteSpace(model.ConfirmNewPassword))
            {
                ModelState.AddModelError("ChangePasswordModel.ConfirmNewPassword", "Xác nhận mật khẩu là bắt buộc");
            }
            else if (model.NewPassword != model.ConfirmNewPassword)
            {
                ModelState.AddModelError("ChangePasswordModel.ConfirmNewPassword", "Mật khẩu xác nhận không khớp");
            }

            if (!ModelState.IsValid)
            {
                // Return view with validation errors - QUAN TRỌNG: return CustomProfileViewModel
                viewModel.Customer = customer;
                viewModel.ChangePasswordModel.Customer = customer;
                viewModel.ChangePasswordModel.HasPassword = hasExistingPassword;
                viewModel.RestaurantOwners = viewModel.RestaurantOwners ?? new List<RestaurantOwner>();

                return View(viewModel); // Return CustomProfileViewModel, không phải ChangePasswordViewModel
            }

            // Hash new password - sử dụng hashService đã khai báo ở trên
            string hashedNewPassword = hashService.HashPassword(model.NewPassword);
            // Update customer information with hashed password
            var updateData = new
            {
                cus_id = customer.cus_id,
                cus_username = customer.cus_username,
                cus_password = hashedNewPassword,  // Update new hashed password
                cus_name = customer.cus_name,
                cus_email = customer.cus_email,
                cus_phone = customer.cus_phone,
                cus_address = customer.cus_address,
                cus_birthday = customer.cus_birthday,
                cus_gender = customer.cus_gender,
                cus_image = customer.cus_image,
                cus_isKYI = customer.cus_is_kyc
            };

            var response = await _apiService.PutAsync<object, dynamic>($"{ApiEndpoints.CUSTOMER}/{cus_id}", updateData);

            if (response != null)
            {
                string successMessage = hasExistingPassword ?
                    NotificationConstants.CHANGE_PASSWORD_SUCCESS :
                    "Tạo mật khẩu thành công!";

                TempData[KeyConstants.SUCCESS_MESSAGE] = successMessage;
                return RedirectToAction(ControllerConstants.CHANGE_PASSWORD);
            }
            else
            {
                string errorMessage = hasExistingPassword ?
                    NotificationConstants.CHANGE_PASSWORD_FAILED :
                    "Tạo mật khẩu thất bại!";

                TempData[KeyConstants.ERROR_MESSAGE] = errorMessage;
                return RedirectToAction(ControllerConstants.CHANGE_PASSWORD);
            }
        }
        /// <summary>
        /// Displays the customer's delivery tracking page.
        /// </summary>
        public async Task<IActionResult> TrackingDelivery()
        {
            int? cus_id = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            if (cus_id == null)
            {
                TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.YOU_NOT_LOGIN;
                return RedirectToAction(ControllerConstants.LOGIN, ControllerConstants.AUTH);
            }

            var customer = await _apiService.GetAsync<Customer>($"{ApiEndpoints.CUSTOMER}/{cus_id}");
            var restaurantOwners = await _apiService.GetAsync<List<RestaurantOwner>>(
                string.Format(ApiEndpoints.RESTAURANT_OWNER_BY_CUS_ID, cus_id)
            );
            var orders = await _apiService.GetAsync<List<Order>>($"{ApiEndpoints.ORDER}/customer/{cus_id}");

            // 👇 Không cần tính lại voucher nữa vì đã có trong order_total
            // Order.order_total đã bao gồm tất cả tính toán (subtotal - voucher + delivery fee)
            
            orders = orders.OrderByDescending(o => o.order_date).ToList();
            
            var viewModel = new CustomProfileViewModel
            {
                Customer = customer,
                RestaurantOwners = restaurantOwners,
                ListOrders = orders
            };

            return View(viewModel);
        }

        public async Task<IActionResult> OrderDetails(int orderId)
        {
            var orderDetails = await _apiService.GetAsync<CustomViewOrderDetails>($"{ApiEndpoints.ORDER}/details/{orderId}");

            // Lấy voucher nếu có mã voucher
            if (!string.IsNullOrEmpty(orderDetails.Order.voucher_code_applied))
            {
                var voucher = await _apiService.GetAsync<Voucher>(
                    $"{ApiEndpoints.VOUCHER}/code/{orderDetails.Order.voucher_code_applied}"
                );
                orderDetails.Voucher = voucher;
            }

            return View(orderDetails);
        }
        // 1. Trang voucher đã sở hữu
        public async Task<IActionResult> VoucherOwned()
        {
            var cus_id = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            if (cus_id == null) return RedirectToAction("Login", "Auth");

            var owned = await _pointService.GetOwnedVouchersAsync(cus_id.Value);
            // ✅ Check hết hạn điểm
            CustomerPoint? point = null;
            try
            {
                point = await _pointService.GetPointAsync(cus_id.Value);
            }
            catch
            {
                // API trả về text "Customer not found" → parse fail
                // => Cho point = null để bỏ qua
                point = null;
            }

            if (point != null && point.point_balance > 0 && point.last_updated.AddDays(365) < DateTime.Now)
            {
                await _pointService.UpdatePointsAsync(new CustomerPointRequest
                {
                    CusId = cus_id.Value,
                    ChangeAmount = -point.point_balance,
                    Description = "Điểm đã hết hạn sau 365 ngày không sử dụng"
                });

                owned.CustomerBalance = 0;
            }

            // Lấy profile model cho layout
            var profileVM = await _customerService.GetProfileViewModel(cus_id.Value);

            // Nhét dữ liệu voucher vào Data
            profileVM.Data["VoucherOwned"] = owned;

            return View("VoucherOwned", profileVM);
        }

        // 2. Trang lịch sử điểm
        public async Task<IActionResult> History()
        {
            var cus_id = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            if (cus_id == null) return RedirectToAction("Login", "Auth");

            var history = await _pointService.GetHistoryAsync(cus_id.Value);

            var profileVM = await _customerService.GetProfileViewModel(cus_id.Value);
            profileVM.Data["History"] = history;

            return View("History", profileVM);
        }

        // 3. Trang đổi điểm lấy voucher
        public async Task<IActionResult> Redeem()
        {
            var cus_id = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            if (cus_id == null) return RedirectToAction("Login", "Auth");

            var available = await _pointService.GetAvailableVouchersAsync(cus_id.Value);

            var profileVM = await _customerService.GetProfileViewModel(cus_id.Value);
            profileVM.Data["Redeem"] = available;

            return View("Redeem", profileVM);
        }

        // POST: Redeem voucher
        [HttpPost]
        public async Task<IActionResult> RedeemVoucher(int VoucherId)
        {
            var cus_id = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            if (cus_id == null) return RedirectToAction("Login", "Auth");

            var request = new RedeemVoucherRequest
            {
                CusId = cus_id.Value,
                VoucherId = VoucherId
            };

            try
            {
                var msg = await _pointService.RedeemVoucherAsync(request);
                TempData["SuccessMessage"] = msg;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Redeem");
        }

        // POST: Gift voucher
        [HttpPost]
        public async Task<IActionResult> GiftVoucher(GiftVoucherRequest request)
        {
            request.SenderId = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID) ?? 0;

            try
            {
                var msg = await _pointService.GiftVoucherAsync(request);
                TempData["SuccessMessage"] = msg;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("VoucherOwned");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteVoucher(int voucherId)
        {
            var cus_id = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            if (cus_id == null) return RedirectToAction("Login", "Auth");

            try
            {
                var msg = await _pointService.DeleteCustomerVoucherAsync(cus_id.Value, voucherId);
                TempData["SuccessMessage"] = msg;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("VoucherOwned");
        }

    }
}
