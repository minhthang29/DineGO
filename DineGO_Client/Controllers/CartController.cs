using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Constant;
using Core.Services;
using Core.Models;
using Core.Common;
using Core.Models.Client.Custom;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Core.Helper;

namespace DineGO_Client.Controllers
{
    /// <summary>
    /// Controller that handles actions related to the customer's shopping cart UI.
    /// </summary>
    /// <author>KhoiNV,sieuhdd</author>
    public class CartController : Controller
    {
        private readonly ILogger<CartItemViewModel> _logger;
        private readonly CartService _cartService;
        private readonly CustomerPointService _pointService;
        private readonly GeoHelper _geoHelper;
        /// <summary>
        /// Constructor to inject services.
        /// </summary>
        public CartController(ILogger<CartItemViewModel> logger, CartService cartService, CustomerPointService pointService, GeoHelper geoHelper)
        {
            _logger = logger;
            _cartService = cartService;
            _pointService = pointService;
            _geoHelper = geoHelper;
        }

        /// <summary>
        /// Displays the cart page for the logged-in customer.
        /// </summary>
        /// <returns>The cart view.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int? cusId = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            if (cusId == null)
                return RedirectToAction("Login", "Auth");

            var model = await _cartService.GetCartItems(cusId.Value);
            return View(model);
        }

        /// <summary>
        /// Deletes a cart item by its cart food ID.
        /// </summary>
        /// <param name="cartFoodId">The ID of the cart food item to remove.</param>
        /// <returns>Redirects to cart index.</returns>
        [HttpPost]
        public async Task<IActionResult> Delete(int cartFoodId)
        {
            await _cartService.DeleteCartItem(cartFoodId);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Updates the quantity of a specific cart item.
        /// </summary>
        /// <param name="cartFoodId">The ID of the cart food item.</param>
        /// <param name="quantity">The new quantity.</param>
        /// <returns>Redirects to cart index.</returns>
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartFoodId, int quantity)
        {
            await _cartService.UpdateQuantity(cartFoodId, quantity);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Displays the checkout confirmation page.
        /// </summary>
        /// <returns>The checkout view.</returns>
        /// <author>KhoiNV,ThanhDT</author>
        [HttpPost]
        public async Task<IActionResult> CheckOut(List<int> SelectedCartFoodIds)
        {
            int? cusId = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            if (cusId == null)
                return RedirectToAction("Login", "Auth");

            var checkoutInfo = await _cartService.GetCheckOutInfoAsync(cusId.Value, SelectedCartFoodIds);

            // Lấy voucher customer đang sở hữu
            var ownedVouchers = await _pointService.GetOwnedVouchersAsync(cusId.Value);
            ViewBag.OwnedVouchers = ownedVouchers?.Vouchers
                .Where(v => v.Quantity > 0
                         && !v.VoucherIsDeleted
                         && v.VoucherIsActive
                         && v.VoucherEndDate >= DateTime.Now)
                .ToList();
            try
            {
                ViewBag.DistanceKm = (checkoutInfo?.Restaurant?.res_latitude.HasValue == true 
                    && checkoutInfo?.Restaurant?.res_longitude.HasValue == true
                    && checkoutInfo?.Customer?.cus_latitude.HasValue == true 
                    && checkoutInfo?.Customer?.cus_longitude.HasValue == true)
                    ? _geoHelper.CalculateDistanceKm(
                        checkoutInfo.Customer.cus_latitude.Value,
                        checkoutInfo.Customer.cus_longitude.Value,
                        checkoutInfo.Restaurant.res_latitude.Value,
                        checkoutInfo.Restaurant.res_longitude.Value)
                    : 0.01;
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Không thể tính khoảng cách giao hàng: {ex.Message}");
                ViewBag.DistanceKm = 0.01; // Fallback
            }
            return View(checkoutInfo);
        }

        /// <summary>
        /// Adds a food item to the customer's cart with a default quantity of 1.
        /// </summary>
        /// <param name="foodId">The ID of the food item to add.</param>
        /// <param name="quantity">Quantity to add (default is 1).</param>
        /// <returns>Redirects to food listing page with success/fail message.</returns>
        /// <author>KhoiNV</author>
        [HttpPost]
        public async Task<IActionResult> Add(int foodId, int quantity = 1)
        {
            int? cusId = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            if (cusId == null) throw new UnauthorizedAccessException();

            var success = await _cartService.AddFoodToCartAsync(cusId.Value, foodId, quantity);

            if (success)
            {

                TempData[KeyConstants.SUCCESS_MESSAGE] = NotificationConstants.ADD_CART_SUCCESS;
                return RedirectToAction("Index", "Food");
            }
            else
            {
                TempData[KeyConstants.ERROR_MESSAGE] = NotificationConstants.ADD_CART_FAIL;
                return RedirectToAction("Index", "Food");
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddJson([FromBody] AddToCartRequest request)
        {
            int? cusId = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            if (cusId == null)
                return Unauthorized(new { message = "Bạn cần đăng nhập" });

            var success = await _cartService.AddFoodToCartAsync(cusId.Value, request.FoodId, request.Quantity);

            if (success)
            {
                return Ok(new { success = true, message = "Thêm thành công" });
            }

            return BadRequest(new { success = false, message = "Không thể thêm vào giỏ" });
        }

        public class AddToCartRequest
        {
            public int FoodId { get; set; }
            public int Quantity { get; set; }
        }

        /// <summary>
        /// Submits the selected cart items for checkout.
        /// Marks items as bought and creates corresponding order.
        /// </summary>
        /// <param name="SelectedCartFoodIds">List of selected cart food IDs to checkout.</param>
        /// <returns>Redirects to checkout page with success or error message.</returns>
        [HttpPost]
        public async Task<IActionResult> SubmitCheckout(List<int> SelectedCartFoodIds, string SelectedVoucherCode, decimal DeliveryFee, DateTime EstimatedDeliveryTime)
        {
            int? cusId = HttpContext.Session.GetInt32(SessionConstants.CUSTOMER_ID);
            if (cusId == null)
                return RedirectToAction("Login", "Auth");

            // Lấy thông tin checkout để tạo đơn hàng
            var checkoutInfo = await _cartService.GetCheckOutInfoAsync(cusId.Value, SelectedCartFoodIds);
            
            // Tính toán các giá trị
            decimal subtotal = checkoutInfo.SelectedFoods.Sum(x => x.Price * x.Quantity);
            decimal voucherDiscountAmount = 0;
            int? voucherType = null;
            decimal? voucherOriginalValue = null;
            
            // Xử lý voucher nếu có
            if (!string.IsNullOrEmpty(SelectedVoucherCode))
            {
                var ownedVouchers = await _pointService.GetOwnedVouchersAsync(cusId.Value);
                var selectedVoucher = ownedVouchers?.Vouchers.FirstOrDefault(v => v.VoucherCode == SelectedVoucherCode);
                
                if (selectedVoucher != null)
                {
                    voucherType = selectedVoucher.VoucherType;
                    voucherOriginalValue = selectedVoucher.VoucherDiscount;
                    
                    if (selectedVoucher.VoucherType == 0) // Voucher %
                    {
                        voucherDiscountAmount = subtotal * selectedVoucher.VoucherDiscount / 100;
                        var maxCap = Math.Min(subtotal * 0.2m, 300000m); // Giới hạn 20% hoặc 300k
                        if (voucherDiscountAmount > maxCap) voucherDiscountAmount = maxCap;
                    }
                    else // Voucher fixed
                    {
                        voucherDiscountAmount = selectedVoucher.VoucherDiscount;
                        var maxCap = subtotal * 0.4m; // Giới hạn 40% đơn hàng
                        if (voucherDiscountAmount > maxCap) voucherDiscountAmount = maxCap;
                    }
                    
                    // Làm tròn lên hàng chục nghìn
                    voucherDiscountAmount = Math.Ceiling(voucherDiscountAmount / 10000) * 10000;
                }
            }
            
            decimal finalTotal = subtotal - voucherDiscountAmount + DeliveryFee;
            
            // Tạo đơn hàng với thông tin cố định
            var orderData = new 
            {
                cus_id = cusId.Value,
                res_id = checkoutInfo.SelectedFoods.First().RestaurantId, // Giả sử tất cả món cùng nhà hàng
                order_date = DateTime.Now,
                order_status = 0, // Chờ xác nhận
                order_subtotal = subtotal,
                delivery_fee = DeliveryFee,
                order_price_discount = voucherDiscountAmount,
                voucher_code_applied = SelectedVoucherCode,
                voucher_type_applied = voucherType,
                voucher_original_value = voucherOriginalValue,
                order_total = finalTotal,
                estimated_delivery_time = EstimatedDeliveryTime,
                
                // Snapshot thông tin khách hàng
                customer_name_snapshot = checkoutInfo.Customer.cus_name,
                customer_phone_snapshot = checkoutInfo.Customer.cus_phone,
                delivery_address_snapshot = checkoutInfo.Customer.cus_address,
                
                // Chi tiết đơn hàng với thông tin cố định
                orderDetails = checkoutInfo.SelectedFoods.Select(food => new 
                {
                    cart_id = food.CartFoodId,
                    food_id = food.FoodId,
                    order_quantity = food.Quantity,
                    order_price = food.Price, // Giá tại thời điểm đặt
                    food_name_snapshot = food.FoodName,
                    food_price_snapshot = food.Price,
                    food_image_snapshot = food.FoodImage,
                    prep_time_snapshot = food.PrepTime
                }).ToList()
            };
            
            // Gửi đến API để tạo đơn hàng
            var success = await _cartService.CreateOrderWithDetailsAsync(orderData);
            
            if (success)
            {
                // Sử dụng voucher nếu có
                if (!string.IsNullOrEmpty(SelectedVoucherCode))
                {
                    await _pointService.UseVoucherAsync(cusId.Value, SelectedVoucherCode);
                }
                
                // Xóa các item đã checkout khỏi cart
                await _cartService.ClearSelectedCartItemsAsync(SelectedCartFoodIds);
                
                TempData["SuccessMessage"] = "Đặt hàng thành công!";
                return RedirectToAction("TrackingDelivery", "Customer");
            }

            TempData["ErrorMessage"] = "Có lỗi xảy ra khi thanh toán.";
            return RedirectToAction("Index");
        }
    }
}
